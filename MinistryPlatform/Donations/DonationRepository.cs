using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using log4net;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Repositories
{
    public class DonationRepository : MinistryPlatformBase, IDonationRepository
    {

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DonationRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public MpDonation GetDonationByTransactionCode(string transactionCode)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();

            var filter = $"Transaction_Code = '{transactionCode}'";
            var donations = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpDonation>();

            if(!donations.Any())
            {
                // TODO possibly refactor to create a more custom exception
                //throw new Exception($"Donation does not exist for transaction code: {transactionCode}");
                _logger.Error($"Donation does not exist for transaction code: {transactionCode}");
                return null;
            }

            return donations.First();
        }

        public List<MpDonation> Update(List<MpDonation> donations)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();
            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Update(donations);
        }

        public MpDonation Update(MpDonation donation)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();
            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Update(donation);
        }

        public MpDonor CreateDonor(MpDonor donor)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();
            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Create(donor);
        }

        public MpDonorAccount CreateDonorAccount(MpDonorAccount donor)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();
            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Create(donor);
        }

        public void UpdateDonorAccount(JObject donorAccount)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();

            try
            {
                MpRestBuilder.NewRequestBuilder()
                    .WithAuthenticationToken(token)
                    .Build()
                    .Update(donorAccount, "Donor_Accounts");
            }
            catch (Exception e)
            {
                _logger.Error($"UpdateRecurringGift: Error updating recurring gift: {JsonConvert.SerializeObject(donorAccount)}", e);
            }
        }

        public MpContactDonor GetContactDonor(int contactId)
        {
            MpContactDonor donor;
            try
            {
                var token = ApiUserRepository.GetDefaultApiClientToken();
                var parameters = new Dictionary<string, object>
                {
                    { "@Contact_ID", contactId }
                };

                var filter = $"api_crds_Get_Contact_Donor";
                var storedProcReturn = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpContactDonor>();

                if (storedProcReturn != null && storedProcReturn.Count > 0)
                    donor = storedProcReturn[0];
                else
                {
                    donor = new MpContactDonor
                    {
                        ContactId = contactId,
                        RegisteredUser = true
                    };
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    string.Format("GetDonorRecord failed.  Contact Id: {0}", contactId), ex);
            }

            return donor;
        }

        public List<MpRecurringGift> GetRecurringGifts(int contactId)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();

            var columns = new string[] {
                "Recurring_Gifts.[Recurring_Gift_ID]",
                "Donor_ID_Table_Contact_ID_Table.[Contact_ID]",
                "Donor_ID_Table.[Donor_ID]",
                "Donor_Account_ID_Table.[Donor_Account_ID]",
                "Frequency_ID_Table.[Frequency_ID]",
                "Recurring_Gifts.[Day_Of_Month]",
                "Day_Of_Week_ID_Table.[Day_Of_Week_ID]",
                "Recurring_Gifts.[Amount]",
                "Recurring_Gifts.[Start_Date]",
                "Recurring_Gifts.[End_Date]",
                "Program_ID_Table.[Program_ID]",
                "Congregation_ID_Table.[Congregation_ID]",
                "Recurring_Gifts.[Subscription_ID]",
                "Recurring_Gifts.[Consecutive_Failure_Count]",
                "Recurring_Gifts.[Source_Url]",
                "Recurring_Gifts.[Predefined_Amount]",
                "Recurring_Gifts.[Vendor_Detail_Url]"
            };

            var filter = $"Donor_ID_Table_Contact_ID_Table.[Contact_ID] = {contactId}";

            return MpRestBuilder.NewRequestBuilder()
                                .WithSelectColumns(columns)
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpRecurringGift>().ToList();
        }

        public List<MpDonation> GetDonations(int contactId)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();

            var columns = new string[] {
                "Donations.[Donation_ID]",
                "Donor_ID_Table_Contact_ID_Table.[Contact_ID]",
                "Donations.[Donation_Amount]",
                "Donation_Status_ID_Table.[Donation_Status_ID]",
                "Donations.[Donation_Status_Date]",
                "Batch_ID_Table.[Batch_ID]",
                "Donations.[Transaction_Code]"
            };

            var filter = $"Donor_ID_Table_Contact_ID_Table.[Contact_ID] = {contactId}";

            return MpRestBuilder.NewRequestBuilder()
                                .WithSelectColumns(columns)
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpDonation>().ToList();
        }

        public List<MpDonationHistory> GetDonationHistoryByContactId(int contactId)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();

            var selectColumns = new string[] {
                "Donation_Distributions.[Donation_ID]",
                "Donation_Distributions.[Donation_Distribution_ID]",
                "Donation_ID_Table.[Donation_Status_Date]",
                "Program_ID_Table.[Program_Name]",
                "Donation_ID_Table.[Donation_Status_ID]",
                "Donation_Distributions.[Amount]",
                "Donation_ID_Table.[Donation_Date]",
                "Donation_ID_Table_Donation_Status_ID_Table.[Donation_Status]"
            };

            var filter = $"Donation_ID_Table_Donor_ID_Table_Contact_ID_Table.[Contact_ID] = {contactId}";

            return MpRestBuilder.NewRequestBuilder()
                .WithSelectColumns(selectColumns)
                .WithAuthenticationToken(token)
                .WithFilter(filter)
                .OrderBy("Donation_ID_Table.[Donation_Date] DESC")
                .Build()
                .Search<MpDonationHistory>().ToList();
        }
    }
}
