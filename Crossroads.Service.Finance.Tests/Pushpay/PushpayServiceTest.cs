using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Services;
using Moq;
using Pushpay.Client;
using Pushpay.Models;
using Xunit;

namespace Crossroads.Service.Finance.Test.Pushpay
{
    public class PushpayServiceTest
    {
        private readonly Mock<IPushpayClient> _pushpayClient;
        private readonly Mock<IMapper> _mapper;

        private readonly IPushpayService _fixture;

        public PushpayServiceTest()
        {
            _pushpayClient = new Mock<IPushpayClient>();
            _mapper = new Mock<IMapper>();

            _fixture = new PushpayService(_pushpayClient.Object, _mapper.Object);
        }

        [Fact]
        public void ShouldGetDepositsByDateRange()
        {
            // Arrange
            var startDate = new DateTime(2017, 12, 12);
            var endDate = new DateTime(2017, 12, 17);

            var pushpayDepositDtos = new List<PushpaySettlementDto>
            {
                new PushpaySettlementDto()
            };

            var depositDtos = new List<DepositDto>
            {
                new DepositDto()
            };

            _mapper.Setup(m => m.Map<List<DepositDto>>(It.IsAny<List<PushpaySettlementDto>>())).Returns(depositDtos);
            _pushpayClient.Setup(m => m.GetDepositByDateRange(startDate, endDate)).Returns(pushpayDepositDtos);

            // Act
            var result = _fixture.GetDepositsByDateRange(startDate, endDate);

            // Assert
            Assert.NotNull(result);
        }
    }
}
