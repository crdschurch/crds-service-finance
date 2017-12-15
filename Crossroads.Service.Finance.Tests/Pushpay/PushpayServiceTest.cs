using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services;
using MinistryPlatform.Interfaces;
using Moq;
using Xunit;
using Pushpay.Client;
using Crossroads.Web.Common.Configuration;
using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Test.Pushpay
{
    public class PushpayServiceTest
    {
        private readonly Mock<IPushpayClient> _pushpayClient;
        private readonly Mock<IDonationService> _donationService;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IConfigurationWrapper> _configurationWrapper;

        private readonly IPushpayService _fixture;

        public PushpayServiceTest()
        {
            _pushpayClient = new Mock<IPushpayClient>();
            _donationService = new Mock<IDonationService>();
            _mapper = new Mock<IMapper>();
            _configurationWrapper = new Mock<IConfigurationWrapper>();

            _fixture = new PushpayService(_pushpayClient.Object, _donationService.Object, _mapper.Object, _configurationWrapper.Object);
        }

        [Fact]
        public void ShouldUpdateDonationStatusPendingFromPushpay()
        {
            string transactionCode = "87234354pending";
            var webhookMock = Mock.PushpayStatusChangeRequestMock.Create();
            _pushpayClient.Setup(r => r.GetPayment(webhookMock)).Returns(Mock.PushpayPaymentDtoMock.CreateProcessing());
            _donationService.Setup(r => r.GetDonationByTransactionCode(It.IsAny<string>())).Returns(Mock.DonationDtoMock.CreatePending(transactionCode));

            var result = _fixture.UpdateDonationStatusFromPushpay(webhookMock);

            // is pending
            Assert.Equal(1, result.DonationStatusId);
        }

        [Fact]
        public void ShouldUpdateDonationStatusSuccessFromPushpay()
        {
            string transactionCode = "87234354v";
            var webhookMock = Mock.PushpayStatusChangeRequestMock.Create();
            _pushpayClient.Setup(r => r.GetPayment(webhookMock)).Returns(Mock.PushpayPaymentDtoMock.CreateSuccess());
            _donationService.Setup(r => r.GetDonationByTransactionCode(It.IsAny<string>())).Returns(Mock.DonationDtoMock.CreatePending(transactionCode));

            var result = _fixture.UpdateDonationStatusFromPushpay(webhookMock);

            // is success
            Assert.Equal(4, result.DonationStatusId);
        }

        [Fact]
        public void ShouldUpdateDonationStatusDeclinedFromPushpay()
        {
            string transactionCode = "87234354v";
            var webhookMock = Mock.PushpayStatusChangeRequestMock.Create();
            _pushpayClient.Setup(r => r.GetPayment(webhookMock)).Returns(Mock.PushpayPaymentDtoMock.CreateFailed());
            _donationService.Setup(r => r.GetDonationByTransactionCode(It.IsAny<string>())).Returns(Mock.DonationDtoMock.CreatePending(transactionCode));

            var result = _fixture.UpdateDonationStatusFromPushpay(webhookMock);

            // is failed
            Assert.Equal(3, result.DonationStatusId);
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

            var depositDtos = new List<SettlementEventDto>
            {
                new SettlementEventDto()
            };

            _mapper.Setup(m => m.Map<List<SettlementEventDto>>(It.IsAny<List<PushpaySettlementDto>>())).Returns(depositDtos);
            _pushpayClient.Setup(m => m.GetDepositsByDateRange(startDate, endDate)).Returns(pushpayDepositDtos);

            // Act
            var result = _fixture.GetDepositsByDateRange(startDate, endDate);

            // Assert
            Assert.NotNull(result);
        }
    }    
}
