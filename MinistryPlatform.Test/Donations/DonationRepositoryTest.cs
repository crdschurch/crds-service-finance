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
using Mock;

namespace MinistryPlatform.Test.Donations
{
    public class DonationRepositoryTest
    {
        readonly Mock<IApiUserRepository> _apiUserRepository;
        readonly Mock<IConfigurationWrapper> _configurationWrapper;
        readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        readonly Mock<IMinistryPlatformRestRequest> _request;
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
            _request = new Mock<IMinistryPlatformRestRequest>(MockBehavior.Strict);

            _apiUserRepository.Setup(r => r.GetDefaultApiClientToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

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
            _apiUserRepository.Setup(r => r.GetDefaultApiClientToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDonation>()).Returns(mpDonations);

            // Act
            var result = _fixture.GetDonationByTransactionCode(transactionCode);

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

            _request.Setup(m => m.Update(It.IsAny<List<MpDonation>>(), null)).Returns(mpDonations);

            // Act
            var result = _fixture.Update(mpDonations);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void ShouldUpdateDonorAccount()
        {
            _request.Setup(m => m.Update(It.IsAny<JObject>(), "Donor_Accounts"));
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
            _apiUserRepository.Setup(r => r.GetDefaultApiClientToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(String.Join(" AND ", filters))).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);            

            _request.Setup(m => m.Search<MpRecurringGift>()).Returns(MpRecurringGiftMock.CreateList(contactId));

            // Act
            var responseRecurringGift = _fixture.GetRecurringGifts(contactId);

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
            _apiUserRepository.Setup(r => r.GetDefaultApiClientToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(String.Join(" AND ", filters))).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpRecurringGift>()).Returns(MpRecurringGiftMock.CreateEmptyList());

            // Act
            var responseRecurringGift = _fixture.GetRecurringGifts(contactId);

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
            _apiUserRepository.Setup(r => r.GetDefaultApiClientToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDonation>()).Returns(MpDonationsMock.CreateList());

            // Act
            var responseDonations = _fixture.GetDonations(contactId);

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
            _apiUserRepository.Setup(r => r.GetDefaultApiClientToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDonation>()).Returns(MpDonationsMock.CreateEmpty());

            // Act
            var responseDonations = _fixture.GetDonations(contactId);

            // Assert
            Assert.Empty(responseDonations);
        }

        [Fact]
        public void ShouldGetDonationHistoriesByContactId()
        {
            // Arrange
            var contactId = 1234567;

            var mpDonationHistories = new List<MpDonationHistory>
            {
                new MpDonationHistory
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
                "Donation_ID_Table_Donation_Status_ID_Table.[Donation_Status]"
            };

            var filter = "Donation_ID_Table_Donor_ID_Table_Contact_ID_Table.[Contact_ID] = 1234567";
            filter += $" AND Donation_ID_Table.[Donation_Date] >= '2018-07-09'";
            var order = "Donation_ID_Table.[Donation_Date] DESC";

            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.OrderBy(order)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDonationHistory>()).Returns(mpDonationHistories);

            // Act
            var result = _fixture.GetDonationHistoryByContactId(contactId, new DateTime(2018, 7, 9));

            // Assert
            Assert.NotNull(result);
        }
    }
}
