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
        const int contactId = 7344;

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
                "Recurring_Gifts.[Recurring_Gift_ID] AS [Recurring Gift ID]",
                "Donor_ID_Table_Contact_ID_Table.[Contact_ID] AS [Contact ID]",
                "Donor_ID_Table.[Donor_ID] AS [Donor ID]",
                "Donor_Account_ID_Table.[Donor_Account_ID] AS [Donor Account ID]",
                "Frequency_ID_Table.[Frequency_ID] AS [Frequency ID]",
                "Recurring_Gifts.[Day_Of_Month] AS [Day Of Month]",
                "Day_Of_Week_ID_Table.[Day_Of_Week_ID] AS [Day Of Week ID]",
                "Recurring_Gifts.[Amount] AS [Amount]",
                "Recurring_Gifts.[Start_Date] AS [Start Date]",
                "Recurring_Gifts.[End_Date] AS [End Date]",
                "Program_ID_Table.[Program_ID] AS [Program ID]",
                "Congregation_ID_Table.[Congregation_ID] AS [Congregation ID]",
                "Recurring_Gifts.[Subscription_ID] AS [Subscription ID]",
                "Recurring_Gifts.[Consecutive_Failure_Count] AS [Consecutive Failure Count]",
                "Recurring_Gifts.[Source_Url] AS [Source Url]",
                "Recurring_Gifts.[Predefined_Amount] AS [Predefined Amount]",
                "Recurring_Gifts.[Vendor_Detail_Url] AS [Vendor Detail Url]"
            };
            var filter = "Donor_ID_Table_Contact_ID_Table.[Contact_ID] = 7344";
            _apiUserRepository.Setup(r => r.GetDefaultApiClientToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
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
                "Recurring_Gifts.[Recurring_Gift_ID] AS [Recurring Gift ID]",
                "Donor_ID_Table_Contact_ID_Table.[Contact_ID] AS [Contact ID]",
                "Donor_ID_Table.[Donor_ID] AS [Donor ID]",
                "Donor_Account_ID_Table.[Donor_Account_ID] AS [Donor Account ID]",
                "Frequency_ID_Table.[Frequency_ID] AS [Frequency ID]",
                "Recurring_Gifts.[Day_Of_Month] AS [Day Of Month]",
                "Day_Of_Week_ID_Table.[Day_Of_Week_ID] AS [Day Of Week ID]",
                "Recurring_Gifts.[Amount] AS [Amount]",
                "Recurring_Gifts.[Start_Date] AS [Start Date]",
                "Recurring_Gifts.[End_Date] AS [End Date]",
                "Program_ID_Table.[Program_ID] AS [Program ID]",
                "Congregation_ID_Table.[Congregation_ID] AS [Congregation ID]",
                "Recurring_Gifts.[Subscription_ID] AS [Subscription ID]",
                "Recurring_Gifts.[Consecutive_Failure_Count] AS [Consecutive Failure Count]",
                "Recurring_Gifts.[Source_Url] AS [Source Url]",
                "Recurring_Gifts.[Predefined_Amount] AS [Predefined Amount]",
                "Recurring_Gifts.[Vendor_Detail_Url] AS [Vendor Detail Url]"
            };
            var filter = "Donor_ID_Table_Contact_ID_Table.[Contact_ID] = 123";
            _apiUserRepository.Setup(r => r.GetDefaultApiClientToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpRecurringGift>()).Returns(MpRecurringGiftMock.CreateEmptyList());

            // Act
            var responseRecurringGift = _fixture.GetRecurringGifts(123);

            // Assert
            Assert.Empty(responseRecurringGift);
        }
    }
}
