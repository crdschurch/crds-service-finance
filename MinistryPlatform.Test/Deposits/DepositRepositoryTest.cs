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

namespace MinistryPlatform.Test.Deposits
{
    public class DepositRepositoryTest
    {
        readonly Mock<IApiUserRepository> _apiUserRepository;
        readonly Mock<IConfigurationWrapper> _configurationWrapper;
        readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        readonly Mock<IMinistryPlatformRestRequest> _request;
        readonly Mock<IMapper> _mapper;

        private readonly IDepositRepository _fixture;

        private string token = "123abc";

        public DepositRepositoryTest()
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

            _fixture = new DepositRepository(_restRequestBuilder.Object,
                _apiUserRepository.Object,
                _configurationWrapper.Object,
                _mapper.Object);
        }

        [Fact]
        public void ShouldCreateDeposit()
        {
            // Arrange
            var processorTransferId = "1a2b3c";

            var mpDeposit = new MpDeposit
            {
                ProcessorTransferId = processorTransferId
            };

            var newMpDeposit = new MpDeposit
            {
                Id = 123,
                ProcessorTransferId = processorTransferId
            };

            _request.Setup(m => m.Create(It.IsAny<MpDeposit>(), null)).Returns(newMpDeposit);

            // Act
            var result = _fixture.CreateDeposit(mpDeposit);

            // Assert
            Assert.Equal(processorTransferId, result.ProcessorTransferId);
        }


        [Fact]
        public void ShouldGetDepositsByTransferIds()
        {
            // Arrange
            var transferIds = new List<string>
            {
                "111aaa222bbb",
                "333ccc444ddd"
            };

            var filter = $"Processor_Transfer_ID IN (" + string.Join(',', transferIds) + ")";
            _apiUserRepository.Setup(r => r.GetDefaultApiUserToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDeposit>()).Returns(new List<MpDeposit>());

            // Act
            var result = _fixture.GetDepositsByTransferIds(transferIds);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldGetDepositsByDepositName()
        {
            var hourDateTime = new DateTime(2018, 01, 04, 13, 15, 00);

            var hourDateTimeString = (hourDateTime.Hour < 10) ? "0" + hourDateTime.Hour : hourDateTime.Hour.ToString();
            var minuteDateTimeString = (hourDateTime.Minute < 10) ? "0" + hourDateTime.Minute : hourDateTime.Minute.ToString();


            var x = hourDateTimeString + minuteDateTimeString;


            // Arrange
            var depositName = "ACH20180103";

            List<MpDeposit> deposits = new List<MpDeposit>
            {
                new MpDeposit
                {
                    DepositName = "ACH20180103"
                }
            };

            var filter = $"Deposit_Name LIKE '%{depositName}%'";
            _apiUserRepository.Setup(r => r.GetDefaultApiUserToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpDeposit>()).Returns(deposits);

            // Act
            var result = _fixture.GetDepositNamesByDepositName(depositName);

            // Assert
            Assert.NotNull(result);
        }
    }
}
