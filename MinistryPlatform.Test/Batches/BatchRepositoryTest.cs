using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Repositories;
using MinistryPlatform.Models;
using Moq;
using Xunit;

namespace MinistryPlatform.Test.Batches
{
    public class BatchRepositoryTest
    {
        readonly Mock<IApiUserRepository> _apiUserRepository;
        readonly Mock<IConfigurationWrapper> _configurationWrapper;
        readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        readonly Mock<IMinistryPlatformRestRequestAsync> _request;
        readonly Mock<IMapper> _mapper;

        private readonly IBatchRepository _fixture;

        private string token = "123abc";

        public BatchRepositoryTest()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _restRequestBuilder = new Mock<IMinistryPlatformRestRequestBuilderFactory>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);
            _restRequest = new Mock<IMinistryPlatformRestRequestBuilder>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _request = new Mock<IMinistryPlatformRestRequestAsync>();

            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);

            _fixture = new BatchRepository(_restRequestBuilder.Object,
                _apiUserRepository.Object,
                _configurationWrapper.Object,
                _mapper.Object);
        }

        [Fact]
        public void ShouldCreateBatch()
        {
            // Arrange
            var processorTransferId = "1a2b3c";

            var mpBatch = new MpDonationBatch
            {
                ProcessorTransferId = processorTransferId
            };

            var newMpBatch = new MpDonationBatch
            {
                Id = 123,
                ProcessorTransferId = processorTransferId
            };

            _request.Setup(m => m.Create(It.IsAny<MpDonationBatch>(), null)).Returns(Task.FromResult(newMpBatch));

            // Act
            var result = _fixture.CreateDonationBatch(mpBatch).Result;

            // Assert
            Assert.Equal(processorTransferId, result.ProcessorTransferId);
        }
    }
}
