﻿using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Congregations;
using MinistryPlatform.Donors;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pushpay.Client;
using Pushpay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services
{
    public class PushpayService : IPushpayService
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IPushpayClient _pushpayClient;
        private readonly IDonationService _donationService;
        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly IWebhooksRepository _webhooksRepository;
        private readonly IGatewayService _gatewayService;
        private readonly IMapper _mapper;
        private readonly IDonationDistributionRepository _donationDistributionRepository;
        private readonly ICongregationRepository _congregationRepository;
        private readonly IConfigurationWrapper _configurationWrapper;

        private readonly int _mpDonationStatusPending, _mpDonationStatusDeposited, _mpDonationStatusDeclined, _mpDonationStatusSucceeded,
                             _mpPushpayRecurringWebhookMinutes, _mpDefaultContactDonorId, _mpNotSiteSpecificCongregationId;
        private const int MaxRetryMinutes = 15;
        private const int PushpayProcessorTypeId = 1;
        private const int NotSiteSpecificCongregationId = 5;
        private readonly string CongregationFieldKey = Environment.GetEnvironmentVariable("PUSHPAY_SITE_FIELD_KEY");

        private Dictionary<string, int> _recurringGiftStatuses = new Dictionary<string, int>
        {
            { "Active", 1 },
            { "Paused", 2 },
            { "Cancelled", 3 }
        };

        public PushpayService(IPushpayClient pushpayClient, IDonationService donationService, IMapper mapper,
                              IConfigurationWrapper configurationWrapper, IRecurringGiftRepository recurringGiftRepository,
                              IProgramRepository programRepository, IContactRepository contactRepository, IDonorRepository donorRepository,
                              IWebhooksRepository webhooksRepository, IGatewayService gatewayService, IDonationDistributionRepository donationDistributionRepository,
                              ICongregationRepository congregationRepository)
        {
            _pushpayClient = pushpayClient;
            _donationService = donationService;
            _mapper = mapper;
            _recurringGiftRepository = recurringGiftRepository;
            _programRepository = programRepository;
            _contactRepository = contactRepository;
            _donorRepository = donorRepository;
            _webhooksRepository = webhooksRepository;
            _gatewayService = gatewayService;
            _donationDistributionRepository = donationDistributionRepository;
            _congregationRepository = congregationRepository;
            _configurationWrapper = configurationWrapper;
            _mpDonationStatusPending = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusPending") ?? 1;
            _mpDonationStatusDeposited = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusDeposited") ?? 2;
            _mpDonationStatusDeclined = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusDeclined") ?? 3;
            _mpDonationStatusSucceeded = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusSucceeded") ?? 4;
            _mpDefaultContactDonorId = configurationWrapper.GetMpConfigIntValue("COMMON", "defaultDonorID") ?? 1;
            _mpPushpayRecurringWebhookMinutes = configurationWrapper.GetMpConfigIntValue("CRDS-FINANCE", "PushpayJobDelayMinutes") ?? 1;
            _mpNotSiteSpecificCongregationId = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "NotSiteSpecific") ?? 5;
        }

        public List<PaymentDto> GetDonationsForSettlement(string settlementKey)
        {
            var result = _pushpayClient.GetDonations(settlementKey);
            return _mapper.Map<List<PaymentDto>>(result);
        }

        public async Task<List<SettlementEventDto>> GetDepositsByDateRange(DateTime startDate, DateTime endDate)
        {
            var result = await _pushpayClient.GetDepositsByDateRange(startDate, endDate);
            return _mapper.Map<List<SettlementEventDto>>(result);
        }

        public async Task<RecurringGiftDto> CreateRecurringGift(PushpayWebhook webhook, int? congregationId)
        {
            var pushpayRecurringGift = await _pushpayClient.GetRecurringGift(webhook.Events[0].Links.RecurringPayment);

            _logger.Info($"Creating recurring gift {pushpayRecurringGift.PaymentToken}");

            var viewRecurringGiftDto = new PushpayLinkDto
            {
                Href = webhook.Events.First().Links.ViewRecurringPayment
            };

            var merchantViewRecurringGiftDto = new PushpayLinkDto
            {
                Href = webhook.Events.First().Links.ViewMerchantRecurringPayment
            };

            pushpayRecurringGift.Links = new PushpayLinksDto
            {
                ViewRecurringPayment = viewRecurringGiftDto,
                MerchantViewRecurringPayment = merchantViewRecurringGiftDto
            };
            var mpRecurringGift = await BuildAndCreateNewRecurringGift(pushpayRecurringGift, congregationId);
            return _mapper.Map<RecurringGiftDto>(mpRecurringGift);
        }

        public async Task<RecurringGiftDto> UpdateRecurringGift(PushpayWebhook webhook, int? congregationId)
        {
            var updatedPushpayRecurringGift = await _pushpayClient.GetRecurringGift(webhook.Events[0].Links.RecurringPayment);
            var existingMpRecurringGift = await _recurringGiftRepository.FindRecurringGiftBySubscriptionId(updatedPushpayRecurringGift.PaymentToken);

            congregationId = await LookupCongregationId(updatedPushpayRecurringGift.PushpayFields, updatedPushpayRecurringGift.Campus.Key);

            if (congregationId == NotSiteSpecificCongregationId)
            { 
                _logger.Info($"No selected site for recurring gift {updatedPushpayRecurringGift.PaymentToken}");
            }

            existingMpRecurringGift.CongregationId = congregationId.GetValueOrDefault();

            var status = updatedPushpayRecurringGift.Status;
            if (status == "Active")
            {
                var updatedMpRecurringGift = await BuildUpdateRecurringGift(existingMpRecurringGift, updatedPushpayRecurringGift);

                // vendor detail url is not available in the pushpay api when getting a recurring gift
                updatedMpRecurringGift.Add(new JProperty("Vendor_Detail_URL", webhook.Events[0].Links.ViewRecurringPayment));
                _recurringGiftRepository.UpdateRecurringGift(updatedMpRecurringGift);
                var updatedDonorAccount = BuildUpdateDonorAccount(existingMpRecurringGift, updatedPushpayRecurringGift);
                _donationService.UpdateDonorAccount(updatedDonorAccount);
            }
            else if (status == "Cancelled" || status == "Paused")
            {
                var updatedMpRecurringGift = BuildEndDatedRecurringGift(existingMpRecurringGift, updatedPushpayRecurringGift);

                // vendor detail url is not available in the pushpay api when getting a recurring gift
                updatedMpRecurringGift.Add(new JProperty("Vendor_Detail_URL", webhook.Events[0].Links.ViewRecurringPayment));
                _recurringGiftRepository.UpdateRecurringGift(updatedMpRecurringGift);
            }
            return _mapper.Map<RecurringGiftDto>(existingMpRecurringGift);
        }

        public async Task<RecurringGiftDto> UpdateRecurringGiftForSync(PushpayRecurringGiftDto pushpayRecurringGift,
            MpRecurringGift mpRecurringGift)
        {
            var congregationId = await LookupCongregationId(pushpayRecurringGift.PushpayFields, pushpayRecurringGift.Campus.Key);

            if (congregationId == NotSiteSpecificCongregationId)
            {
                _logger.Info($"No selected site for recurring gift {pushpayRecurringGift.PaymentToken}");
            }

            var status = pushpayRecurringGift.Status;
            if (status == "Active")
            {
                var buildUpdateRecurringGiftTask =
                    Task.Run(() => BuildUpdateRecurringGift(mpRecurringGift, pushpayRecurringGift));
                var updatedMpRecurringGift = await buildUpdateRecurringGiftTask;

                _recurringGiftRepository.UpdateRecurringGift(updatedMpRecurringGift);

                var updatedDonorAccountTask = 
                    Task.Run(() => BuildUpdateDonorAccount(mpRecurringGift, pushpayRecurringGift));
                var updatedDonorAccount = await updatedDonorAccountTask;

                _donationService.UpdateDonorAccount(updatedDonorAccount);
            }
            else if (status == "Cancelled" || status == "Paused")
            {
                var updatedMpRecurringGiftTask = Task.Run(() => BuildEndDatedRecurringGift(mpRecurringGift, pushpayRecurringGift));
                var updatedMpRecurringGift = await updatedMpRecurringGiftTask;
                _recurringGiftRepository.UpdateRecurringGift(updatedMpRecurringGift);
            }

            return _mapper.Map<RecurringGiftDto>(mpRecurringGift);
        }

        private JObject BuildEndDatedRecurringGift(MpRecurringGift mpRecurringGift, PushpayRecurringGiftDto updatedPushpayRecurringGift)
        {
            var mappedMpRecurringGift = _mapper.Map<MpRecurringGift>(updatedPushpayRecurringGift);

            var updateGift = new JObject(
                new JProperty("Recurring_Gift_ID", mpRecurringGift.RecurringGiftId),
                new JProperty("End_Date", DateTime.Now),
                new JProperty("Recurring_Gift_Status_ID", GetRecurringGiftStatusId(mappedMpRecurringGift.Status)),
                new JProperty("Updated_On", updatedPushpayRecurringGift.UpdatedOn),
                new JProperty("Status_Changed_Date", System.DateTime.Now),
                new JProperty("Congregation_ID", mpRecurringGift.CongregationId)
            );

            return updateGift;
        }

        private async Task<JObject> BuildUpdateRecurringGift(MpRecurringGift mpRecurringGift, PushpayRecurringGiftDto updatedPushpayRecurringGift)
        {
            var mappedMpRecurringGift = _mapper.Map<MpRecurringGift>(updatedPushpayRecurringGift);

            var updateGift = new JObject( 
                new JProperty("Recurring_Gift_ID", mpRecurringGift.RecurringGiftId),
                new JProperty("Amount", mappedMpRecurringGift.Amount),
                new JProperty("Frequency_ID", mappedMpRecurringGift.FrequencyId),
                new JProperty("Day_Of_Month", mappedMpRecurringGift.DayOfMonth),
                new JProperty("Day_Of_Week_ID", mappedMpRecurringGift.DayOfWeek),
                new JProperty("Start_Date", mappedMpRecurringGift.StartDate),
                new JProperty("Program_ID", (await _programRepository.GetProgramByName(updatedPushpayRecurringGift.Fund.Code)).ProgramId),
                new JProperty("End_Date", null),
                new JProperty("Recurring_Gift_Status_ID", GetRecurringGiftStatusId(mappedMpRecurringGift.Status)),
                new JProperty("Updated_On", updatedPushpayRecurringGift.UpdatedOn),
                new JProperty("Congregation_ID", mpRecurringGift.CongregationId)
            );

            if (mpRecurringGift.RecurringGiftStatusId != _recurringGiftStatuses[updatedPushpayRecurringGift.Status])
            {
                updateGift.Add(new JProperty("Status_Changed_Date", System.DateTime.Now));
            }

            return updateGift;
        }

        private JObject BuildUpdateDonorAccount(MpRecurringGift mpRecurringGift, PushpayRecurringGiftDto updatedPushpayRecurringGift)
        {
            var mpDonorAccount = MapDonorAccountPaymentDetails(updatedPushpayRecurringGift);
            return new JObject(
                new JProperty("Donor_Account_ID", mpRecurringGift.DonorAccountId),
                new JProperty("Account_Number", mpDonorAccount.AccountNumber),
                new JProperty("Routing_Number", mpDonorAccount.RoutingNumber),
                new JProperty("Institution_Name", mpDonorAccount.InstitutionName),
                new JProperty("Account_Type_ID", mpDonorAccount.AccountTypeId),
                new JProperty("Processor_ID", mpDonorAccount.ProcessorId)
            );
        }

        public async Task<MpRecurringGift> BuildAndCreateNewRecurringGift (PushpayRecurringGiftDto pushpayRecurringGift, int? congregationId)
        {
            var mpRecurringGift = _mapper.Map<MpRecurringGift>(pushpayRecurringGift);
            var mpDonor = await FindOrCreateDonorAndDonorAccount(pushpayRecurringGift);

            mpRecurringGift.DonorId = mpDonor.DonorId.Value;
            mpRecurringGift.DonorAccountId = mpDonor.DonorAccountId.Value;

            congregationId = await LookupCongregationId(pushpayRecurringGift.PushpayFields, pushpayRecurringGift.Campus.Key);

            if (congregationId == NotSiteSpecificCongregationId)
            {
                _logger.Info($"No selected site for recurring gift {pushpayRecurringGift.PaymentToken}");
            }

            mpRecurringGift.CongregationId = congregationId.GetValueOrDefault();

            mpRecurringGift.ConsecutiveFailureCount = 0;
            mpRecurringGift.ProgramId = (await _programRepository.GetProgramByName(pushpayRecurringGift.Fund.Code)).ProgramId;
            mpRecurringGift.RecurringGiftStatusId = MpRecurringGiftStatus.Active;
            mpRecurringGift.UpdatedOn = pushpayRecurringGift.UpdatedOn;

            var mpRecurringGiftNotesTask = Task.Run(() => GetRecurringGiftNotes(pushpayRecurringGift));
            mpRecurringGift.Notes = await mpRecurringGiftNotesTask;
            

            // note: this is normally set when the recurring gift is created via the webhook, but can be set here when the recurring gifts sync. Pushpay
            // does not currently send over the view recurring gift link except during the webhook, so this code will not populate the user view link until 
            // they add it to their api call
            if (pushpayRecurringGift.Links.ViewRecurringPayment != null && String.IsNullOrEmpty(mpRecurringGift.VendorDetailUrl))
            {
                mpRecurringGift.VendorDetailUrl = pushpayRecurringGift.Links.ViewRecurringPayment.Href;
            }

            if (pushpayRecurringGift.Links.MerchantViewRecurringPayment != null && String.IsNullOrEmpty(mpRecurringGift.VendorAdminDetailUrl))
            {
                mpRecurringGift.VendorAdminDetailUrl = pushpayRecurringGift.Links.MerchantViewRecurringPayment.Href;
            }

            if (pushpayRecurringGift.PushpayFields!= null && pushpayRecurringGift.PushpayFields.Any(r => r.Key == CongregationFieldKey))
            {
                var congregationName = Helpers.Helpers.Translate(pushpayRecurringGift.PushpayFields.First(r => r.Key == CongregationFieldKey).Value);
                var congregations = await _congregationRepository.GetCongregationByCongregationName(congregationName);

                if (congregations.Any())
                {
                    mpRecurringGift.CongregationId = congregations.First(r => r.CongregationName == congregationName)
                        .CongregationId;
                }
                else
                {
                    _logger.Info($"Site mismatch - {congregationName} not found in MP.");
                }
            }
            else
            {
                _logger.Info($"No selected site for recurring gift {pushpayRecurringGift.PaymentToken}");
            }

            mpRecurringGift = await _recurringGiftRepository.CreateRecurringGift(mpRecurringGift);

            // STRIPE CANCELLATION - this can be removed after there are no more Stripe recurring gifts
            // This cancels a Stripe gift if a subscription id was uploaded to Pushpay (i.e. through pushpay migration tool)
            if (pushpayRecurringGift.Notes != null && pushpayRecurringGift.Notes.Trim().StartsWith("sub_", StringComparison.Ordinal))
            {
                _gatewayService.CancelStripeRecurringGift(pushpayRecurringGift.Notes.Trim());
            }

            // This cancels all Stripe gifts on the donor that are the same program
            var mpRecurringGifts = await _recurringGiftRepository.FindRecurringGiftsByDonorId((int)mpDonor.DonorId);
            if (mpRecurringGifts != null && mpRecurringGifts.Count > 0)
            {
                foreach (MpRecurringGift gift in mpRecurringGifts)
                {
                    if (gift.EndDate == null && gift.SubscriptionId.StartsWith("sub_")
                        && gift.ProgramName.ToLower().Trim() == pushpayRecurringGift.Fund.Name.ToLower().Trim())
                    {
                        _gatewayService.CancelStripeRecurringGift(gift.SubscriptionId);
                    }
                }
            }
            // END STRIPE CANCELLATION section

            return mpRecurringGift;
        }

        // this formats +15134567788 to (513) 456-7788 
        public string FormatPhoneNumber(string phone)
        {
            string area = phone.Substring(2, 3);
            string major = phone.Substring(5, 3);
            string minor = phone.Substring(8);
            return string.Format("({0}) {1}-{2}", area, major, minor);
        }

        public string GetRecurringGiftNotes(PushpayRecurringGiftDto pushpayRecurringGift)
        {
            var payer = pushpayRecurringGift.Payer;
            var address = payer.Address;
            var notes = new List<string>
            {
                $"First Name: {payer.FirstName}",
                $"Last Name: {payer.LastName}",
                $"Phone: " + (!String.IsNullOrEmpty(payer.MobileNumber) ? FormatPhoneNumber(payer.MobileNumber) : ""),
                $"Email: {payer.EmailAddress}",
                "Address1: " + (!String.IsNullOrEmpty(address.AddressLine1) ? address.AddressLine1 : "Street Address Not Provided"),
                $"Address2: " + (!String.IsNullOrEmpty(address.AddressLine2) ? address.AddressLine2 : ""),
                $"City, State Zip: {address.City}, {address.State} {address.Zip}",
                // pushpay sets the country as USA on donations but only gives us US here
                $"Country: " + (address.Country == "US" ? "USA" : address.Country)
            };
            return string.Join(" ", notes);
        }

        private async Task<MpDonor> FindOrCreateDonorAndDonorAccount(PushpayRecurringGiftDto gift)
        {
            var donorId = await _donorRepository.GetDonorIdByProcessorId(gift.Payer.Key);
            if (donorId != null) {
                var existingMatchedDonor = await _donorRepository.GetDonorByDonorId(donorId.GetValueOrDefault());
                // we found a matching donor by processor id (i.e. we have previously matched them)
                //   create a new donor account on donor for this recurring gift
                existingMatchedDonor.DonorAccountId = (await GetOrCreateDonorAccount(gift, existingMatchedDonor.DonorId.Value)).DonorAccountId;
                return existingMatchedDonor;
            }
            // we didn't match a donor with a processor id (i.e. previously matched), so let's
            //   run the same stored proc that PushPay uses to attempt to match
            var matchedContact = await _contactRepository.MatchContact(gift.Payer.FirstName, gift.Payer.LastName,
                                            gift.Payer.MobileNumber, gift.Payer.EmailAddress);

            if (matchedContact != null)
            {
                // contact was matched
                if (matchedContact.DonorId == null)
                {
                    // matched contact did not have a donor record,
                    //   so create and attach donor to contact
                    var mpDonor = new MpDonor()
                    {
                        ContactId = matchedContact.ContactId,
                        StatementFrequencyId = 1, // quarterly
                        StatementTypeId = 1, // individual
                        StatementMethodId = 2, // email+online
                        SetupDate = DateTime.Now
                    };
                    matchedContact.DonorId = (await _donationService.CreateDonor(mpDonor)).DonorId;
                }

                // create donor account and attach to contact
                matchedContact.DonorAccountId = (await CreateDonorAccount(gift, matchedContact.DonorId.Value)).DonorAccountId;
                return matchedContact;
            } else {
                // donor not matched, assign to default contact
                var donorAccount = await CreateDonorAccount(gift, _mpDefaultContactDonorId);
                var mpDoner = new MpDonor()
                {
                    DonorId = _mpDefaultContactDonorId,
                    DonorAccountId = donorAccount.DonorAccountId,
                    CongregationId = _mpNotSiteSpecificCongregationId
                };
                return mpDoner;
            }
        }

        private MpDonorAccount MapDonorAccountPaymentDetails(PushpayTransactionBaseDto basePushpayTransaction, int? donorId = null)
        {
            var isBank = basePushpayTransaction.PaymentMethodType.ToLower() == "ach";
            var mpDonorAccount = new MpDonorAccount()
            {
                AccountNumber = isBank ? basePushpayTransaction.Account.Reference : basePushpayTransaction.Card.Reference,
                InstitutionName = isBank ? basePushpayTransaction.Account.BankName : GetCardBrand(basePushpayTransaction.Card.Brand),
                RoutingNumber = isBank ? basePushpayTransaction.Account.RoutingNumber : null,
                NonAssignable = false,
                DomainId = 1,
                Closed = false,
                ProcessorId = basePushpayTransaction.Payer.Key,
                ProcessorTypeId = PushpayProcessorTypeId
            };
            if (donorId != null) {
                mpDonorAccount.DonorId = donorId.Value;
            }
            // set account type
            switch (basePushpayTransaction.PaymentMethodType.ToLower())
            {
                case "ach":
                    if (basePushpayTransaction.Account.AccountType == "Checking")
                    {
                        mpDonorAccount.AccountTypeId = MpAccountTypes.Checkings;
                    }
                    else if (basePushpayTransaction.Account.AccountType == "Savings")
                    {
                        mpDonorAccount.AccountTypeId = MpAccountTypes.Savings;
                    }
                    break;
                case "creditcard":
                    mpDonorAccount.AccountTypeId = MpAccountTypes.CreditCard;
                    break;
            }
            return mpDonorAccount;
        }

        private string GetCardBrand(string pushpayCardBrand)
        {
            switch (pushpayCardBrand) {
                case "VISA":
                    return "Visa";
                case "Discover":
                    return "Discover";
                case "Amex":
                    return "AmericanExpress";
                case "MasterCard":
                    return "MasterCard";
                default:
                    return "";
            }
        }

        private async Task<MpDonorAccount> CreateDonorAccount(PushpayTransactionBaseDto basePushpayTransaction, int donorId)
        {
            var mpDonorAccount = MapDonorAccountPaymentDetails(basePushpayTransaction, donorId);
            return await _donationService.CreateDonorAccount(mpDonorAccount);
        }

        private async Task<MpDonorAccount> GetOrCreateDonorAccount(PushpayTransactionBaseDto basePushPayTransaction,
            int donorId)
        {
            var mpDonorAccount = MapDonorAccountPaymentDetails(basePushPayTransaction, donorId);
            var mpCurrentDonorAccounts = await _donationService.GetDonorAccounts(donorId);

            var matchingDonorAccounts = mpCurrentDonorAccounts.Where(da => 
                da.Closed == mpDonorAccount.Closed 
                && da.NonAssignable == mpDonorAccount.NonAssignable
                && da.AccountNumber == mpDonorAccount.AccountNumber
                && da.DonorId == mpDonorAccount.DonorId
                && da.InstitutionName == mpDonorAccount.InstitutionName
                && da.ProcessorId == mpDonorAccount.ProcessorId
                && da.RoutingNumber == mpDonorAccount.RoutingNumber
                && da.AccountTypeId == mpDonorAccount.AccountTypeId
                && da.ProcessorTypeId == mpDonorAccount.ProcessorTypeId).ToList();

            if (matchingDonorAccounts.Count > 0)
            {
                return matchingDonorAccounts[0];
            }
            else
            {
                return await _donationService.CreateDonorAccount(mpDonorAccount);
            }
        }

        public int GetRecurringGiftStatusId(string recurringGiftStatus)
        {
            switch (recurringGiftStatus)
            {
                case PushpayRecurringGiftStatus.Active:
                    return MpRecurringGiftStatus.Active;
                case PushpayRecurringGiftStatus.Paused:
                    return MpRecurringGiftStatus.Paused;
                case PushpayRecurringGiftStatus.Cancelled:
                    return MpRecurringGiftStatus.Cancelled;
                default:
                    return 0;
            }
        }

        public List<PushpayRecurringGiftDto> GetRecurringGiftsByDateRange(DateTime startDate, DateTime endDate)
        {
            var pushpayRecurringGiftDtos = _pushpayClient.GetNewAndUpdatedRecurringGiftsByDateRange(startDate, endDate);
            return pushpayRecurringGiftDtos;
        }

        public void SaveWebhookData(PushpayWebhook pushpayWebhook)
        {
            try
            {
                string eventType = null;
                if (pushpayWebhook?.Events?.Count == 1)
                    eventType = pushpayWebhook.Events[0].EventType;

                MpPushpayWebhook webhookData = new MpPushpayWebhook()
                {
                    EventType = eventType,
                    Payload = JsonConvert.SerializeObject(pushpayWebhook, Formatting.None)
                };

                _webhooksRepository.Create(webhookData);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in PushpayService.SaveWebhookData: {ex.Message}");
            }
        }

        public async Task<int> LookupCongregationId(List<PushpayFieldValueDto> pushpayFields, string campusKey)
        {
	        var lookupCongregationId = 0;

            if (pushpayFields != null && pushpayFields.Any(r => r.Key == CongregationFieldKey))
            {
                var congregationName = Helpers.Helpers.Translate(pushpayFields.First(r => r.Key == CongregationFieldKey).Value);

                // TODO: consider caching these values on application startup
                var congregations = await _congregationRepository.GetCongregationByCongregationName(congregationName);

                if (congregations.Any())
                {
                    lookupCongregationId = congregations.First(r => r.CongregationName == congregationName).CongregationId;
                }
            }
            else
            {
                // get the pushpay campus key here
                lookupCongregationId = (await _configurationWrapper.GetMpConfigIntValueAsync("CRDS-FINANCE", campusKey)).GetValueOrDefault();
            }

            if (lookupCongregationId == 0)
            {
                lookupCongregationId = NotSiteSpecificCongregationId;
            }

            return lookupCongregationId;
        }
    }
}
