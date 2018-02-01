using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Configuration;
using Hangfire;
using log4net;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Pushpay.Client;
using Pushpay.Models;
using Crossroads.Web.Common.Configuration;

namespace Crossroads.Service.Finance.Services
{
    public class PushpayService : IPushpayService
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IPushpayClient _pushpayClient;
        private readonly IDonationService _donationService;
        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;
        private readonly int _mpDonationStatusPending, _mpDonationStatusDeclined, _mpDonationStatusSucceeded, _mpPushpayRecurringWebhookMinutes,
                            _mpDefaultContactId, _mpDefaultContactDonorId, _mpDefaultCongregationId;
        private const int maxRetryMinutes = 10;
        //private const int defaultContactId = 1;
        //private const int defaultContactDonorId = 1;
        //private const int defaultCongregationId = 1;

        public PushpayService(IPushpayClient pushpayClient, IDonationService donationService, IMapper mapper,
                              IConfigurationWrapper configurationWrapper, IRecurringGiftRepository recurringGiftRepository,
                              IProgramRepository programRepository, IContactRepository contactRepository)
        {
            _pushpayClient = pushpayClient;
            _donationService = donationService;
            _mapper = mapper;
            _recurringGiftRepository = recurringGiftRepository;
            _programRepository = programRepository;
            _contactRepository = contactRepository;
            _mpDonationStatusPending = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusPending") ?? 1;
            _mpDonationStatusDeclined = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusDeclined") ?? 3;
            _mpDonationStatusSucceeded = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusSucceeded") ?? 4;
            _mpDefaultContactDonorId = configurationWrapper.GetMpConfigIntValue("COMMON", "defaultDonorID") ?? 1;
            _mpDefaultCongregationId = configurationWrapper.GetMpConfigIntValue("COMMON", "defaultCongregationID") ?? 1;
            _mpPushpayRecurringWebhookMinutes = configurationWrapper.GetAppMpConfigIntValue("PushpayJobDelayMinutes") ?? 1;
        }

        public PaymentsDto GetChargesForTransfer(string settlementKey)
        {
            var result = _pushpayClient.GetPushpayDonations(settlementKey);
            return _mapper.Map<PaymentsDto>(result);
        }

        private PaymentDto GetPayment(PushpayWebhook webhook)
        {
            var result = _pushpayClient.GetPayment(webhook);
            return _mapper.Map<PaymentDto>(result);
        }

        public void AddUpdateStatusJob(PushpayWebhook webhook)
        {
            // add incoming timestamp so that we can reprocess job for a
            //   certain amount of time
            webhook.IncomingTimeUtc = DateTime.UtcNow;
            AddUpdateDonationStatusFromPushpayJob(webhook);
        }

        private void AddUpdateDonationStatusFromPushpayJob(PushpayWebhook webhook)
        {
            BackgroundJob.Schedule(() => UpdateDonationStatusFromPushpay(webhook, true), TimeSpan.FromMinutes(_mpPushpayRecurringWebhookMinutes));
        }

        public DonationDto UpdateDonationStatusFromPushpay(PushpayWebhook webhook, bool retry=false)
        {
            try {
                var pushpayPayment = _pushpayClient.GetPayment(webhook);
                // PushPay creates the donation a variable amount of time after the webhook comes in
                //   so it still may not be available
                var donation = _donationService.GetDonationByTransactionCode(pushpayPayment.TransactionId);
                if (pushpayPayment.IsStatusNew || pushpayPayment.IsStatusProcessing)
                {
                    donation.DonationStatusId = _mpDonationStatusPending;
                }
                else if (pushpayPayment.IsStatusSuccess)
                {
                    donation.DonationStatusId = _mpDonationStatusSucceeded;

                }
                else if (pushpayPayment.IsStatusFailed)
                {
                    donation.DonationStatusId = _mpDonationStatusDeclined;
                }
                _donationService.Update(donation);
                return donation;
            } catch (Exception e) {
                // donation not created by pushpay yet
                var now = DateTime.UtcNow;
                var webhookTime = webhook.IncomingTimeUtc;
                // if it's been less than ten minutes, try again in a minute
                if ((now - webhookTime).TotalMinutes < maxRetryMinutes && retry)
                {
                    AddUpdateDonationStatusFromPushpayJob(webhook);
                    // dont throw an exception as Hangfire tries to handle it
                    _logger.Error($"Payment: {webhook.Events[0].Links.Payment} not found in MP. Trying again in a minute.", e);
                    return null;
                }
                // it's been more than 10 minutes, let's chalk it up as PushPay
                //   ain't going to create it and call it a day
                else
                {
                    // dont throw an exception as Hangfire tries to handle it
                    _logger.Error($"Payment: {webhook.Events[0].Links.Payment} not found in MP after 10 minutes of trying. Giving up.", e);
                    return null;
                }
            }
        }

        public List<SettlementEventDto> GetDepositsByDateRange(DateTime startDate, DateTime endDate)
        {
            var result = _pushpayClient.GetDepositsByDateRange(startDate, endDate);
            return _mapper.Map<List<SettlementEventDto>>(result);
        }

        public PushpayAnticipatedPaymentDto CreateAnticipatedPayment(PushpayAnticipatedPaymentDto anticipatedPayment)
        {
            return _pushpayClient.CreateAnticipatedPayment(anticipatedPayment);
        }

        public RecurringGiftDto CreateRecurringGift(PushpayWebhook webhook)
        {
            var pushpayRecurringGift = _pushpayClient.GetRecurringGift(webhook.Events[0].Links.RecurringPayment);
            var mpRecurringGift = BuildRecurringGift(pushpayRecurringGift);
            return _mapper.Map<RecurringGiftDto>(mpRecurringGift);
        }

        private MpRecurringGift BuildRecurringGift (PushpayRecurringGiftDto pushpayRecurringGift)
        {
            var mpRecurringGift = _mapper.Map<MpRecurringGift>(pushpayRecurringGift);
            var donor = FindOrCreateDonorAndDonationAccount(pushpayRecurringGift);

            mpRecurringGift.DonorId = donor.DonorId.Value;
            mpRecurringGift.DonorAccountId = donor.DonorAccountId.Value;
            mpRecurringGift.CongregationId = _contactRepository.GetHousehold(donor.HouseholdId).CongregationId;

            mpRecurringGift.ConsecutiveFailureCount = 0;
            mpRecurringGift.DomainId = 1;
            mpRecurringGift.ProgramId = _programRepository.GetProgramByName(pushpayRecurringGift.Fund.Code).ProgramId;

            mpRecurringGift = _recurringGiftRepository.CreateRecurringGift(mpRecurringGift);
            return mpRecurringGift;
        }


        private MpDonor FindOrCreateDonorAndDonationAccount(PushpayRecurringGiftDto gift)
        {
            var existingMatchedDonor = _contactRepository.FindDonorByProcessorId(gift.Payer.Key);
            if (existingMatchedDonor != null && existingMatchedDonor.DonorId.HasValue) {
                // we found a matching donor by processor id (i.e. we have previously matched them)
                //   create a new donor account on donor for this recurring gift
                existingMatchedDonor.DonorAccountId = CreateDonorAccount(gift, existingMatchedDonor.DonorId.Value).DonorAccountId;
                return existingMatchedDonor;
            }
            // we didn't match a donor with a processor id (i.e. previously matched), so let's
            //   run the same stored proc that PushPay uses to attempt to match
            var matchedContact = _contactRepository.MatchContact(gift.Payer.FirstName, gift.Payer.LastName,
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
                        StatementFrequencyId = 2, // annual
                        StatementTypeId = 1, // individual
                        StatementMethodId = 2, // email+online
                        SetupDate = DateTime.Now
                    };
                    matchedContact.DonorId = _donationService.CreateDonor(mpDonor).DonorId;
                }
                // update processor id on donor account so we dont have to manually match next time
                _contactRepository.UpdateProcessor(matchedContact.DonorId.Value, gift.Payer.Key);

                // create donor account and attach to contact
                matchedContact.DonorAccountId = CreateDonorAccount(gift, matchedContact.DonorId.Value).DonorAccountId;
                return matchedContact;
            } else {
                // donor not matched, assign to default contact
                var donorAccount = CreateDonorAccount(gift, _mpDefaultContactDonorId);
                var mpDoner = new MpDonor()
                {
                    DonorId = _mpDefaultContactDonorId,
                    DonorAccountId = donorAccount.DonorAccountId,
                    CongregationId = _mpDefaultCongregationId
                };
                return mpDoner;
            }
        }

        private MpDonorAccount CreateDonorAccount(PushpayRecurringGiftDto gift, int donorId)
        {
            var isBank = gift.Account != null;
            var mpDonorAccount = new MpDonorAccount()
            {
                AccountNumber = isBank ? gift.Account.Reference : gift.Card.Reference,
                InstitutionName = isBank ? gift.Account.BankName : gift.Card.Brand,
                RoutingNumber = isBank ? gift.Account.RoutingNumber : null,
                DonorId = donorId,
                NonAssignable = false,
                DomainId = 1,
                Closed = false
            };
            // set account type
            switch (gift.PaymentMethodType)
            {
                // TODO is this right?
                case "ACH":
                    mpDonorAccount.AccountTypeId = MpAccountTypes.Savings;
                    break;
                case "AchCheck":
                    mpDonorAccount.AccountTypeId = MpAccountTypes.Checkings;
                    break;
                case "CreditCard":
                    mpDonorAccount.AccountTypeId = MpAccountTypes.CreditCard;
                    break;
            }
            return _donationService.CreateDonorAccount(mpDonorAccount);
        }
    }
}
