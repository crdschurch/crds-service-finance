using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private const int PausedRecurringGiftStatusId = 2;

        public DonationRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public async Task<MpDonation> GetDonationByTransactionCode(string transactionCode)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var filter = $"Transaction_Code = '{transactionCode}'";
            var donations = await MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .BuildAsync()
                                .Search<MpDonation>();

            if(!donations.Any())
            {
                _logger.Error($"Donation does not exist for transaction code: {transactionCode}");
                Console.WriteLine($"Donation does not exist for transaction code: {transactionCode}");
                return null;
            }

            return donations.First();
        }

        public async Task<List<MpDonation>> Update(List<MpDonation> donations)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");
            return await MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .BuildAsync()
                .Update(donations);
        }

        public async Task<MpDonation> Update(MpDonation donation)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");
            return await MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .BuildAsync()
                .Update(donation);
        }

        public async Task<MpDonor> CreateDonor(MpDonor donor)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");
            return await MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .BuildAsync()
                .Create(donor);
        }

        public async Task<MpDonorAccount> CreateDonorAccount(MpDonorAccount donor)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");
            return await MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .BuildAsync()
                .Create(donor);
        }

        public async Task<List<MpDonorAccount>> GetDonorAccounts(int donorId)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");
            var columns = new[]
            {
                "Donor_Accounts.[Donor_Account_ID]"
                , "Donor_ID_Table.[Donor_ID]"
                , "Donor_Accounts.[Non-Assignable]"
                , "Account_Type_ID_Table.[Account_Type_ID]"
                , "Donor_Accounts.[Closed]"
                , "Donor_Accounts.[Institution_Name]"
                , "Donor_Accounts.[Account_Number]"
                , "Donor_Accounts.[Routing_Number]"
                , "Donor_Accounts.[Processor_ID]"
                , "Processor_Type_ID_Table.[Processor_Type_ID]"
            };
            var filter = $"Donor_ID_Table.[Donor_ID] = { donorId }";

            return await MpRestBuilder.NewRequestBuilder().WithAuthenticationToken(token).WithSelectColumns(columns)
                .WithFilter(filter).BuildAsync().Search<MpDonorAccount>();
        }

        public void UpdateDonorAccount(JObject donorAccount)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            try
            {
                MpRestBuilder.NewRequestBuilder()
                    .WithAuthenticationToken(token)
                    .Build()
                    .Update(donorAccount, "Donor_Accounts");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"UpdateRecurringGift: Error updating recurring gift: {JsonConvert.SerializeObject(donorAccount)}, {ex.Message}");
                Console.WriteLine($"UpdateRecurringGift: Error updating recurring gift: {JsonConvert.SerializeObject(donorAccount)}, {ex.Message}");
            }
        }

        public async Task<List<MpRecurringGift>> GetRecurringGifts(int contactId)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

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
                "Program_ID_Table.[Program_Name]",
                "Congregation_ID_Table.[Congregation_ID]",
                "Recurring_Gifts.[Subscription_ID]",
                "Recurring_Gifts.[Consecutive_Failure_Count]",
                "Recurring_Gifts.[Source_Url]",
                "Recurring_Gifts.[Predefined_Amount]",
                "Recurring_Gifts.[Vendor_Detail_Url]",
                "Recurring_Gifts.[Recurring_Gift_Status_ID]"
            };

            var filters = new string[] {
                $"Donor_ID_Table_Contact_ID_Table.[Contact_ID] = {contactId}",
                $"(Recurring_Gifts.[End_Date] IS NULL OR Recurring_Gifts.[Recurring_Gift_Status_ID] = {PausedRecurringGiftStatusId})"
            };

            return (await MpRestBuilder.NewRequestBuilder()
                                .WithSelectColumns(columns)
                                .WithAuthenticationToken(token)
                                .WithFilter(String.Join(" AND ", filters))
                                .OrderBy("Recurring_Gifts.[Recurring_Gift_ID] DESC")
                                .BuildAsync()
                                .Search<MpRecurringGift>()).ToList();
        }

        public async Task<List<MpRecurringGift>> GetRecurringGiftsByContactIdAndDates(int contactId, DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

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
                "Program_ID_Table.[Program_Name]",
                "Congregation_ID_Table.[Congregation_ID]",
                "Recurring_Gifts.[Subscription_ID]",
                "Recurring_Gifts.[Consecutive_Failure_Count]",
                "Recurring_Gifts.[Source_Url]",
                "Recurring_Gifts.[Predefined_Amount]",
                "Recurring_Gifts.[Vendor_Detail_Url]",
                "Recurring_Gifts.[Recurring_Gift_Status_ID]"
            };

            var filters = new List<string> {
                $"Donor_ID_Table_Contact_ID_Table.[Contact_ID] = {contactId}",
                $"(Recurring_Gifts.[End_Date] IS NULL OR Recurring_Gifts.[Recurring_Gift_Status_ID] = {PausedRecurringGiftStatusId})"
            };

            if (startDate != null)
            {
                filters.Add($"Recurring_Gifts.[Start_Date] >= '{startDate:yyyy-MM-dd}'");
            }

            if (endDate != null)
            {
                filters.Add($"Recurring_Gifts.[End_Date] <= '{endDate:yyyy-MM-dd}'");
            }

            return (await MpRestBuilder.NewRequestBuilder()
                .WithSelectColumns(columns)
                .WithAuthenticationToken(token)
                .WithFilter(String.Join(" AND ", filters))
                .OrderBy("Recurring_Gifts.[Recurring_Gift_ID] DESC")
                .BuildAsync()
                .Search<MpRecurringGift>()).ToList();
        }

        public async Task<List<MpDonation>> GetDonations(int contactId)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

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

            return (await MpRestBuilder.NewRequestBuilder()
                                .WithSelectColumns(columns)
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .BuildAsync()
                                .Search<MpDonation>()).ToList();
        }

        public async Task<List<MpDonationDetail>> GetDonationHistoryByContactId(int contactId, DateTime? startDate, DateTime? endDate)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var selectColumns = new string[] {
                "Donation_Distributions.[Donation_ID]",
                "Donation_Distributions.[Donation_Distribution_ID]",
                "Donation_ID_Table.[Donation_Status_Date]",
                "Program_ID_Table.[Program_Name]",
                "Donation_ID_Table.[Donation_Status_ID]",
                "Donation_Distributions.[Amount]",
                "Donation_ID_Table.[Donation_Date]",
                "Donation_ID_Table.[Item_Number]",
                "Donation_ID_Table.[Notes]",
                "Donation_ID_Table_Payment_Type_ID_Table.[Payment_Type]",
                "Donation_ID_Table_Payment_Type_ID_Table.[Payment_Type_ID]",
                "Donation_ID_Table_Donation_Status_ID_Table.[Donation_Status]",
                "Donation_ID_Table_Donor_Account_ID_Table.[Account_Number]",
                "Donation_ID_Table_Donor_Account_ID_Table.[Institution_Name]",
                "Donation_ID_Table_Donor_Account_ID_Table.[Routing_Number]",
                "Donation_ID_Table_Donor_Account_ID_Table_Account_Type_ID_Table.[Account_Type]",
                "Donation_ID_Table_Donor_Account_ID_Table_Processor_Type_ID_Table.[Processor_Type]"
            };

            var filters = new List<string> {
                $"Donation_ID_Table_Donor_ID_Table_Contact_ID_Table.[Contact_ID] = {contactId}",
                "Donation_ID_Table_Donation_Status_ID_Table.[Donation_Status] <> 'Offset'"
            };

            if (startDate != null)
            {
                filters.Add($"Donation_ID_Table.[Donation_Date] >= '{startDate:yyyy-MM-dd}'");
            }

            if (endDate != null)
            {
                filters.Add($"Donation_ID_Table.[Donation_Date] <= '{endDate:yyyy-MM-dd}'");
            }

            var order = "Donation_ID_Table.[Donation_Date] DESC";

            return (await MpRestBuilder.NewRequestBuilder()
                .WithSelectColumns(selectColumns)
                .WithAuthenticationToken(token)
                .WithFilter(String.Join(" AND ", filters))
                .OrderBy(order)
                .BuildAsync()
                .Search<MpDonationDetail>()).ToList();
        }

        public async Task<List<MpDonationDetail>> GetOtherGiftsByContactId(int contactId)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var selectColumns = new string[] {
                "Donation_Distributions.[Donation_ID]",
                "Donation_ID_Table.[Donation_Date]",
                "Program_ID_Table.[Program_Name]",
                "Donation_Distributions.[Amount]",
                "Donation_ID_Table_Donor_ID_Table_Contact_ID_Table.[Display_Name]"
            };

            var filter = $"Soft_Credit_Donor_Table_Contact_ID_Table.[Contact_ID] = {contactId}";

            var order = "Donation_ID_Table.[Donation_Date] DESC";

            return (await MpRestBuilder.NewRequestBuilder()
                .WithSelectColumns(selectColumns)
                .WithAuthenticationToken(token)
                .WithFilter(filter)
                .OrderBy(order)
                .BuildAsync()
                .Search<MpDonationDetail>()).ToList();
        }

        public async Task<List<MpDonationDetail>> GetOtherGiftsForRelatedContact(int contactId, DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var selectColumns = new string[] {
                "Donation_Distributions.[Donation_ID]",
                "Donation_ID_Table.[Donation_Date]",
                "Program_ID_Table.[Program_Name]",
                "Donation_Distributions.[Amount]",
                "Donation_ID_Table_Donor_ID_Table_Contact_ID_Table.[Display_Name]"
            };

            var filters = new List<string>
            {
                $"Soft_Credit_Donor_Table_Contact_ID_Table.[Contact_ID] = {contactId}"
            };

            if (startDate != null)
            {
                filters.Add($"Donation_ID_Table.[Donation_Date] >= '{startDate:yyyy-MM-dd}'");
            }

            if (endDate != null)
            {
                filters.Add($"Donation_ID_Table.[Donation_Date] <= '{endDate:yyyy-MM-dd}'");
            }

            var order = "Donation_ID_Table.[Donation_Date] DESC";

            return (await MpRestBuilder.NewRequestBuilder()
                .WithSelectColumns(selectColumns)
                .WithAuthenticationToken(token)
                .WithFilter(String.Join(" AND ", filters))
                .OrderBy(order)
                .BuildAsync()
                .Search<MpDonationDetail>()).ToList();
        }

        public async Task<List<MpDonation>> GetDonationsByTransactionIds(List<string> transactionCodes)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var filter = $"Transaction_Code IN ({string.Join(",", transactionCodes)})";

            return (await MpRestBuilder.NewRequestBuilder()
                //.WithSelectColumns(columns)
                .WithAuthenticationToken(token)
                .WithFilter(filter)
                .BuildAsync()
                .Search<MpDonation>()).ToList();
        }

        public void CreateRawPushpayDonation(string rawDonation)
        {
	        var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

	        var parameters = new Dictionary<string, object>
	        {
		        {"@RawJson", rawDonation}
	        };

	        MpRestBuilder.NewRequestBuilder()
		        .WithAuthenticationToken(token)
		        .Build()
		        .ExecuteStoredProc("api_crds_Insert_PushpayDonationsRawJson", parameters);
        }
    }
}
