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

            _apiUserRepository.Setup(r => r.GetDefaultApiUserToken()).Returns(token);
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
            _apiUserRepository.Setup(r => r.GetDefaultApiUserToken()).Returns(token);
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
                "Recurring_Gift_ID",
                "Contact_ID",
                "Donor_ID",
                "Donor_Account_ID",
                "Frequency_ID",
                "Day_Of_Month",
                "Day_Of_Week_ID",
                "Amount",
                "Start_Date",
                "End_Date",
                "Program_ID",
                "Congregation_ID",
                "Subscription_ID",
                "Consecutive_Failure_Count",
                "Source_Url",
                "Predefined_Amount",
                "Vendor_Detail_Url"
            };
            var filter = "Contact_ID = 7344";
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
        public void GetContactEmpty()
        {
            // Arrange
            var selectColumns = new string[] {
                "Recurring_Gift_ID",
                "Contact_ID",
                "Donor_ID",
                "Donor_Account_ID",
                "Frequency_ID",
                "Day_Of_Month",
                "Day_Of_Week_ID",
                "Amount",
                "Start_Date",
                "End_Date",
                "Program_ID",
                "Congregation_ID",
                "Subscription_ID",
                "Consecutive_Failure_Count",
                "Source_Url",
                "Predefined_Amount",
                "Vendor_Detail_Url"
            };
            var filter = "Contact_ID = 7344";
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
