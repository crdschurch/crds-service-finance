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
    public class RecurringGiftRepositoryTest
    {
        readonly Mock<IApiUserRepository> _apiUserRepository;
        readonly Mock<IConfigurationWrapper> _configurationWrapper;
        readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        readonly Mock<IMinistryPlatformRestRequest> _request;
        readonly Mock<IMapper> _mapper;

        private string token = "123abc";

        private readonly IRecurringGiftRepository _fixture;

        public RecurringGiftRepositoryTest()
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

            _fixture = new RecurringGiftRepository(_restRequestBuilder.Object,
                _apiUserRepository.Object,
                _configurationWrapper.Object,
                _mapper.Object);
        }

        [Fact]
        public void CreateRecurringGift()
        {
            var mpRecurringGift = new MpRecurringGift
            {
                Amount = 12
            };

            _apiUserRepository.Setup(r => r.GetDefaultApiUserToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Create<MpRecurringGift>(It.IsAny<MpRecurringGift>(), null)).Returns(mpRecurringGift);

            var result = _fixture.CreateRecurringGift(mpRecurringGift);

            Assert.Equal(12, result.Amount);
        }
    }
}
