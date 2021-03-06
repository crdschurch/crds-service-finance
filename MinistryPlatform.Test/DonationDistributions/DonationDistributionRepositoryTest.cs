﻿using System;
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

namespace MinistryPlatform.Test.Pledges
{
    public class DonationDistributionRepositoryTest
    {
        readonly Mock<IApiUserRepository> _apiUserRepository;
        readonly Mock<IConfigurationWrapper> _configurationWrapper;
        readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        readonly Mock<IMinistryPlatformRestRequestAsync> _request;
        readonly Mock<IMapper> _mapper;

        private string token = "123abc";
        const int contactId = 7344;

        private readonly IDonationDistributionRepository _fixture;

        public DonationDistributionRepositoryTest()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _restRequestBuilder = new Mock<IMinistryPlatformRestRequestBuilderFactory>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);
            _restRequest = new Mock<IMinistryPlatformRestRequestBuilder>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _request = new Mock<IMinistryPlatformRestRequestAsync>(MockBehavior.Strict);

            _apiUserRepository.Setup(r => r.GetApiClientTokenAsync("CRDS.Service.Finance")).Returns(Task.FromResult(token));
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);

            _fixture = new DonationDistributionRepository(_restRequestBuilder.Object,
                _apiUserRepository.Object,
                _configurationWrapper.Object,
                _mapper.Object);
        }

        [Fact]
        public void GetByPledges()
        {
            var pledgeIds = new int[] { 34, 995 };
            // Arrange
            var selectColumns = new string[] {
                "Pledge_ID_Table.[Pledge_ID]",
                "Donation_Distributions.[Donation_Distribution_ID]",
                "Donation_Distributions.[Amount]"
            };
            var filter = "Pledge_ID_Table.[Pledge_ID] IN (34,995)";
            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDonationDistribution>()).Returns(Task.FromResult(MpDonationDistributionMock.CreateList()));

            // Act
            var response = _fixture.GetByPledges(pledgeIds.ToList()).Result;

            // Assert
            Assert.Equal(3, response.Count);
        }

        [Fact]
        public void ShouldGetDonationDistributionByDonationId()
        {
            // Arrange
            var donationId = 6677889;

            var selectColumns = new string[] {
                "Donation_Distributions.[Donation_Distribution_ID]",
                "Donation_Distributions.[Donation_ID]",
                "Donation_Distributions.[Amount]",
                "Donation_Distributions.[Pledge_ID]",
                "Donation_Distributions.[Congregation_ID]",
                "Donation_Distributions.[HC_Donor_Congregation_ID]"
            };

            var filter = $"Donation_Distributions.[Donation_ID] = {donationId}";

            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDonationDistribution>()).Returns(Task.FromResult(MpDonationDistributionMock.CreateList()));

            // Act
            var result = _fixture.GetByDonationId(donationId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldUpdateDonationDistributions()
        {
            // Arrange
            var mpDonationDistributions = new List<MpDonationDistribution>();

            _request.Setup(m => m.Update(It.IsAny<List<MpDonationDistribution>>(), null, false)).Returns(Task.FromResult(new List<MpDonationDistribution>()));

            // Act
            var result = _fixture.UpdateDonationDistributions(mpDonationDistributions);

            // Assert
            Assert.NotNull(result);
            _request.VerifyAll();
        }
    }
}
