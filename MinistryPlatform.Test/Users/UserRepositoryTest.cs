using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using Crossroads.Web.Common.Security;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using MinistryPlatform.Users;
using Moq;
using Xunit;

namespace MinistryPlatform.Test.Users
{
    public class UserRepositoryTest
    {
        readonly Mock<IApiUserRepository> _apiUserRepository;
        readonly Mock<IConfigurationWrapper> _configurationWrapper;
        readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        readonly Mock<IMinistryPlatformRestRequest> _request;
        readonly Mock<IMapper> _mapper;
        readonly Mock<IAuthenticationRepository> _authenticationRepository;

        private readonly IUserRepository _fixture;

        private string token = "123abc";
        private string clientId = "CRDS.Service.Finance";

        public UserRepositoryTest()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _restRequestBuilder = new Mock<IMinistryPlatformRestRequestBuilderFactory>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);
            _restRequest = new Mock<IMinistryPlatformRestRequestBuilder>(MockBehavior.Strict);
            _authenticationRepository = new Mock<IAuthenticationRepository>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);

            _request = new Mock<IMinistryPlatformRestRequest>();

            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _fixture = new UserRepository(_restRequestBuilder.Object,
                _apiUserRepository.Object,
                _configurationWrapper.Object,
                _mapper.Object,
                _authenticationRepository.Object);
        }

        [Fact]
        public void ShouldGetUserByEmailAddress()
        {
            // Arrange
            var emailAddress = "billy.hamilton@test.com";
            var mpUsers = new List<MpUser>
            {
                new MpUser
                {
                    ContactId = 5544555,
                    UserEmail = "billy.hamilton@test.com",
                    UserId = 6655667
                }
            };

            var columns = new string[] {
                "User_ID",
                "Contact_ID",
                "User_Email"
            };

            _apiUserRepository.Setup(m => m.GetApiClientToken("CRDS.Common")).Returns(token);

            var filter = $"User_Email = '{emailAddress}'";
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(columns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpUser>()).Returns(mpUsers);

            // Act
            var result = _fixture.GetUserByEmailAddress(emailAddress);

            // Assert
            Assert.NotNull(result);
        }
    }
}
