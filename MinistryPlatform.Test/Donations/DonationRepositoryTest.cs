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

        private readonly IDonationRepository _fixture;

        public DonationRepositoryTest()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _restRequestBuilder = new Mock<IMinistryPlatformRestRequestBuilderFactory>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);
            _restRequest = new Mock<IMinistryPlatformRestRequestBuilder>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _request = new Mock<IMinistryPlatformRestRequest>();

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

            _request.Setup(m => m.Update(It.IsAny<List<MpDonation>>())).Returns(mpDonations);

            // Act
            var result = _fixture.UpdateDonations(mpDonations);

            // Assert
            Assert.Single(result);
        }
    }
}
