using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Repositories;
using MinistryPlatform.Models;
using Moq;
using Xunit;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using Mock;

namespace MinistryPlatform.Test.Donations
{
    public class DonationRepositoryTest
    {
        readonly Mock<IApiUserRepository> _apiUserRepository;
        readonly Mock<IConfigurationWrapper> _configurationWrapper;
        readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        readonly Mock<IMinistryPlatformRestRequestAsync> _request;
        readonly Mock<IMapper> _mapper;

        private string token = "123abc";
        private const int contactId = 7344;
        private const int PausedRecurringGiftStatusId = 2;

        private readonly IDonationRepository _fixture;

        public DonationRepositoryTest()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _restRequestBuilder = new Mock<IMinistryPlatformRestRequestBuilderFactory>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);
            _restRequest = new Mock<IMinistryPlatformRestRequestBuilder>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _request = new Mock<IMinistryPlatformRestRequestAsync>(MockBehavior.Strict);

            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);

            _fixture = new DonationRepository(_restRequestBuilder.Object,
                _apiUserRepository.Object,
                _configurationWrapper.Object,
                _mapper.Object);
        }

        [Fact]
        public void GetDonationByTransactionCode()
        {
            // Arrange
            var transactionCode = "zzz111yyy222";

            var mpDonations = new List<MpDonation>
            {
                new MpDonation
                {
                    TransactionCode = transactionCode
                }
            };

            var filter = $"Transaction_Code = '{transactionCode}'";
            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDonation>()).Returns(Task.FromResult(mpDonations));

            // Act
            var result = _fixture.GetDonationByTransactionCode(transactionCode).Result;

            // Assert
            Assert.Equal(transactionCode, result.TransactionCode);
        }

        [Fact]
        public void ShouldUpdateDonations()
        {
            // Arrange
            var mpDonations = new List<MpDonation>
            {
                new MpDonation
                {
                    TransactionCode = "1a"
                }
            };

            _request.Setup(m => m.Update(It.IsAny<List<MpDonation>>(), null, false)).Returns(Task.FromResult(mpDonations));

            // Act
            var result = _fixture.Update(mpDonations).Result;

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void ShouldUpdateDonorAccount()
        {
            _request.Setup(m => m.Update(It.IsAny<JObject>(), "Donor_Accounts", false));
            _fixture.UpdateDonorAccount(new JObject());
        }

        [Fact]
        public void GetRecurringGift()
        {
            // Arrange
            var selectColumns = new string[] {
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
            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(String.Join(" AND ", filters))).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.OrderBy("Recurring_Gifts.[Recurring_Gift_ID] DESC")).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);            

            _request.Setup(m => m.Search<MpRecurringGift>()).Returns(Task.FromResult(MpRecurringGiftMock.CreateList(contactId)));

            // Act
            var responseRecurringGift = _fixture.GetRecurringGifts(contactId).Result;

            // Assert
            Assert.Equal(contactId, responseRecurringGift.FirstOrDefault().ContactId);
        }

        [Fact]
        public void GetRecurringGiftEmpty()
        {
            // Arrange
            var selectColumns = new string[] {
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
            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(String.Join(" AND ", filters))).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.OrderBy("Recurring_Gifts.[Recurring_Gift_ID] DESC")).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpRecurringGift>()).Returns(Task.FromResult(MpRecurringGiftMock.CreateEmptyList()));

            // Act
            var responseRecurringGift = _fixture.GetRecurringGifts(contactId).Result;

            // Assert
            Assert.Empty(responseRecurringGift);
        }

        [Fact]
        public void GetDonation()
        {
            // Arrange
            var selectColumns = new string[] {
                "Donations.[Donation_ID]",
                "Donor_ID_Table_Contact_ID_Table.[Contact_ID]",
                "Donations.[Donation_Amount]",
                "Donation_Status_ID_Table.[Donation_Status_ID]",
                "Donations.[Donation_Status_Date]",
                "Batch_ID_Table.[Batch_ID]",
                "Donations.[Transaction_Code]"
            };
            var filter = "Donor_ID_Table_Contact_ID_Table.[Contact_ID] = 7344";
            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDonation>()).Returns(Task.FromResult(MpDonationsMock.CreateList()));

            // Act
            var responseDonations = _fixture.GetDonations(contactId).Result;

            // Assert
            Assert.Equal(3, responseDonations.Count);
        }

        [Fact]
        public void GetDonationEmpty()
        {
            // Arrange
            var selectColumns = new string[] {
                "Donations.[Donation_ID]",
                "Donor_ID_Table_Contact_ID_Table.[Contact_ID]",
                "Donations.[Donation_Amount]",
                "Donation_Status_ID_Table.[Donation_Status_ID]",
                "Donations.[Donation_Status_Date]",
                "Batch_ID_Table.[Batch_ID]",
                "Donations.[Transaction_Code]"
            };
            var filter = "Donor_ID_Table_Contact_ID_Table.[Contact_ID] = 7344";
            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDonation>()).Returns(Task.FromResult(MpDonationsMock.CreateEmpty()));

            // Act
            var responseDonations = _fixture.GetDonations(contactId).Result;

            // Assert
            Assert.Empty(responseDonations);
        }

        [Fact]
        public void ShouldGetDonationHistoriesByContactId()
        {
            // Arrange
            var contactId = 1234567;

            var mpDonationHistories = new List<MpDonationDetail>
            {
                new MpDonationDetail
                {
                    DonationId = 5544555
                }
            };

            // Arrange
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

            var filter = "Donation_ID_Table_Donor_ID_Table_Contact_ID_Table.[Contact_ID] = 1234567";
            filter += " AND Donation_ID_Table_Donation_Status_ID_Table.[Donation_Status] <> 'Offset'";
            filter += $" AND Donation_ID_Table.[Donation_Date] >= '2018-07-09'";
            var order = "Donation_ID_Table.[Donation_Date] DESC";

            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.OrderBy(order)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDonationDetail>()).Returns(Task.FromResult(mpDonationHistories));

            // Act
            var result = _fixture.GetDonationHistoryByContactId(contactId, new DateTime(2018, 7, 9));

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetDonorAccounts()
        {

            // Arrange
            var donorId = 777790;
            var token = "abc123432afe";
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
            var mpDonorAccount = new MpDonorAccount
            {
                Closed = false,
                AccountNumber = "4894894894",
                InstitutionName = "Bank Of America",
                DomainId = 1,
                DonorId = 777790,
                DonorAccountId = 8098
            };
            
            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(columns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);
            
            _request.Setup(m => m.Search<MpDonorAccount>()).ReturnsAsync(new List<MpDonorAccount>{mpDonorAccount});
            
            // Act
            var results = await _fixture.GetDonorAccounts(donorId);

            // Assert
            Assert.Contains(mpDonorAccount, results);
        }

        [Fact]
        public void ShouldGetDonationsByTransactionIdList()
        {
            // Arrange
            var transactionCodes = new List<string>
            {
                "abc123def456",
                "ghi789jkl012"
            };

            var selectColumns = new string[] {
                "Donations.[Donation_ID]",
                "Donor_ID_Table_Contact_ID_Table.[Contact_ID]",
                "Donations.[Donation_Amount]",
                "Donation_Status_ID_Table.[Donation_Status_ID]",
                "Donations.[Donation_Status_Date]",
                "Batch_ID_Table.[Batch_ID]",
                "Donations.[Transaction_Code]"
            };

            var filter = $"Transaction_Code IN ({string.Join(",", transactionCodes)})";

            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDonation>()).Returns(Task.FromResult(MpDonationsMock.CreateList()));

            // Act
            var results = _fixture.GetDonationsByTransactionIds(transactionCodes).Result;

            // Assert
            Assert.NotNull(results);
        }
    }
}
