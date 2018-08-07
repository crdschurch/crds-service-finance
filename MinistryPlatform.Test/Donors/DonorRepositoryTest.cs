using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using Crossroads.Web.Common.Security;
using MinistryPlatform.Donors;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using MinistryPlatform.Repositories;
using Moq;
using Xunit;

namespace MinistryPlatform.Test.Donors
{
    public class DonorRepositoryTest
    {
        readonly Mock<IApiUserRepository> _apiUserRepository;
        readonly Mock<IConfigurationWrapper> _configurationWrapper;
        readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        readonly Mock<IMinistryPlatformRestRequest> _request;
        readonly Mock<IMapper> _mapper;
        readonly Mock<IAuthenticationRepository> _authenticationRepository;

        private readonly IDonorRepository _fixture;

        private string token = "123abc";
        private string clientId = "CRDS.Service.Finance";
        private int pushpayProcessorType = 1;

        public DonorRepositoryTest()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _restRequestBuilder = new Mock<IMinistryPlatformRestRequestBuilderFactory>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);
            _restRequest = new Mock<IMinistryPlatformRestRequestBuilder>(MockBehavior.Strict);
            _authenticationRepository = new Mock<IAuthenticationRepository>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);

            _request = new Mock<IMinistryPlatformRestRequest>();

            _apiUserRepository.Setup(r => r.GetDefaultApiClientToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _fixture = new DonorRepository(_restRequestBuilder.Object,
                _apiUserRepository.Object,
                _configurationWrapper.Object,
                _mapper.Object,
                _authenticationRepository.Object);
        }

        [Fact]
        public void ShouldFindDonorIdByProcessorId()
        {
            // Arrange
            var processorId = "ABC123def456";

            var mpDonorAccounts = new List<MpDonorAccount>
            {
                new MpDonorAccount
                {
                    DonorId = 123457
                }
            };

            var columns = new string[] {
                "Donor_Accounts.[Donor_Account_ID]",
                "Donor_Accounts.[Donor_ID]",
                "Donor_Accounts.[Non-Assignable]",
                "Donor_Accounts.[Domain_ID]",
                "Donor_Accounts.[Account_Type_ID]",
                "Donor_Accounts.[Closed]",
                "Donor_Accounts.[Institution_Name]",
                "Donor_Accounts.[Account_Number]",
                "Donor_Accounts.[Routing_Number]",
                "Donor_Accounts.[Processor_ID]",
                "Donor_Accounts.[Processor_Type_ID]"
            };

            var filter = $"Processor_ID = '{processorId}' AND Processor_Type_ID = {pushpayProcessorType}";

            _apiUserRepository.Setup(m => m.GetDefaultApiClientToken()).Returns(token);

            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(columns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDonorAccount>()).Returns(mpDonorAccounts);

            // Act
            var result = _fixture.GetDonorIdByProcessorId(processorId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldReturnNullIfNoDonorAccountWithProcessorId()
        {
            // Arrange
            var processorId = "ABC123def456";

            var mpDonorAccounts = new List<MpDonorAccount>
            {

            };

            var columns = new string[] {
                "Donor_Accounts.[Donor_Account_ID]",
                "Donor_Accounts.[Donor_ID]",
                "Donor_Accounts.[Non-Assignable]",
                "Donor_Accounts.[Domain_ID]",
                "Donor_Accounts.[Account_Type_ID]",
                "Donor_Accounts.[Closed]",
                "Donor_Accounts.[Institution_Name]",
                "Donor_Accounts.[Account_Number]",
                "Donor_Accounts.[Routing_Number]",
                "Donor_Accounts.[Processor_ID]",
                "Donor_Accounts.[Processor_Type_ID]"
            };

            var filter = $"Processor_ID = '{processorId}' AND Processor_Type_ID = {pushpayProcessorType}";

            _apiUserRepository.Setup(m => m.GetDefaultApiClientToken()).Returns(token);

            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(columns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDonorAccount>()).Returns(mpDonorAccounts);

            // Act
            var result = _fixture.GetDonorIdByProcessorId(processorId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ShouldGetDonorByDonorId()
        {
            // Arrange
            var donorId = 7766888;

            var donorColumns = new string[] {
                "Donors.[Donor_ID]",
                "Contact_ID_Table.[Contact_ID]",
                "Contact_ID_Table.[Email_Address]",
                "Contact_ID_Table_Household_ID_Table.[Household_ID]"
            };

            var donorFilter = $"Donor_ID = '{donorId}'";

            _apiUserRepository.Setup(m => m.GetApiClientToken(clientId)).Returns(token);

            _apiUserRepository.Setup(r => r.GetDefaultApiClientToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(donorFilter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(donorColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Get<MpDonor>(donorId)).Returns(new MpDonor());

            // Act
            var result = _fixture.GetDonorByDonorId(donorId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldGetNullDonorByDonorId()
        {
            // Arrange
            var donorId = 7766888;

            var donorColumns = new string[] {
                "Donors.[Donor_ID]",
                "Contact_ID_Table.[Contact_ID]",
                "Contact_ID_Table.[Email_Address]",
                "Contact_ID_Table_Household_ID_Table.[Household_ID]"
            };

            var donorFilter = $"Donor_ID = '{donorId}'";

            _apiUserRepository.Setup(m => m.GetApiClientToken(clientId)).Returns(token);

            _apiUserRepository.Setup(r => r.GetDefaultApiClientToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(donorFilter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(donorColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Get<MpDonor>(donorId)).Returns((MpDonor)null);

            // Act
            var result = _fixture.GetDonorByDonorId(donorId);

            // Assert
            Assert.Null(result);
        }
    }
}
