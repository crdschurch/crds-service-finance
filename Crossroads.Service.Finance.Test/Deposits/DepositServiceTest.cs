using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Xunit;
using Moq;
using RestSharp;

namespace Crossroads.Service.Finance.Test.Deposits
{
    public class DepositServiceTest
    {
        private readonly Mock<IDepositRepository> _depositRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IPushpayService> _pushpayService;
        private readonly Mock<IConfigurationWrapper> _configWrapper;
        private readonly string _pushpayWebEndpoint;

        private readonly IDepositService _fixture;

        public DepositServiceTest()
        {
            _depositRepository = new Mock<IDepositRepository>();
            _mapper = new Mock<IMapper>();
            _pushpayService = new Mock<IPushpayService>();
            _configWrapper = new Mock<IConfigurationWrapper>();
            _pushpayWebEndpoint = Environment.GetEnvironmentVariable("PUSHPAY_WEB_ENDPOINT");

            _fixture = new DepositService(_depositRepository.Object, _mapper.Object, _pushpayService.Object, _configWrapper.Object);
        }

        [Fact]
        public void ShouldCreateAndReturnDepositObject()
        {
            // Arrange
            var depositName = "FD20171219";
            var settlementKey = "aaabbb111222";
            var amount = "1000";

            var settlementEventDto = new SettlementEventDto
            {
                Name = depositName,
                EstimatedDepositDate = new DateTime(2018, 02, 03),
                Key = settlementKey,
                TotalAmount = new AmountDto
                {
                    Amount = amount
                }
            };

            var mpDeposits = new List<MpDeposit>
            {
                new MpDeposit(),
                new MpDeposit(),
            };

            _depositRepository.Setup(r => r.GetByName(It.IsAny<string>())).Returns(mpDeposits);

            // Act
            var result = _fixture.CreateDeposit(settlementEventDto);

            // Assert
            Assert.Equal(settlementKey, result.ProcessorTransferId);
            Assert.Equal($"{_pushpayWebEndpoint}/pushpay/0/settlements?includeCardSettlements=True&includeAchSettlements=True&fromDate=02-03-2018&toDate=02-03-2018", result.VendorDetailUrl);
            Assert.Equal(Decimal.Parse(amount), result.DepositTotalAmount);
            Assert.Equal(depositName + "002", result.DepositName);
        }

        [Fact]
        public void ShouldCreateAndReturnDepositObjectWithOverTenDeposits()
        {
            // Arrange
            var depositName = "FD20171219";
            var settlementKey = "aaabbb111222";
            var amount = "1000";

            var settlementEventDto = new SettlementEventDto
            {
                Name = depositName,
                EstimatedDepositDate = new DateTime(2018, 2, 3),
                Key = settlementKey,
                TotalAmount = new AmountDto
                {
                    Amount = amount
                }
            };

            var mpDeposits = new List<MpDeposit>
            {
                new MpDeposit(),
                new MpDeposit(),
                new MpDeposit(),
                new MpDeposit(),
                new MpDeposit(),
                new MpDeposit(),
                new MpDeposit(),
                new MpDeposit(),
                new MpDeposit(),
                new MpDeposit(),
                new MpDeposit(),
            };

            _depositRepository.Setup(r => r.GetByName(It.IsAny<string>())).Returns(mpDeposits);

            // Act
            var result = _fixture.CreateDeposit(settlementEventDto);

            // Assert
            Assert.Equal($"{_pushpayWebEndpoint}/pushpay/0/settlements?includeCardSettlements=True&includeAchSettlements=True&fromDate=02-03-2018&toDate=02-03-2018", result.VendorDetailUrl);
            Assert.Equal(Decimal.Parse(amount), result.DepositTotalAmount);
            Assert.Equal(depositName + "011", result.DepositName);
        }

        [Fact]
        public void ShouldTruncateDepositNameOverFifteenChars()
        {
            // Arrange
            var depositName = "ABCDEFGHIJKLMNO";
            var settlementKey = "aaabbb111222";
            var amount = "1000";

            var settlementEventDto = new SettlementEventDto
            {
                Name = depositName,
                EstimatedDepositDate = new DateTime(2018, 2, 3),
                Key = settlementKey,
                TotalAmount = new AmountDto
                {
                    Amount = amount
                }
            };

            var mpDeposits = new List<MpDeposit>
            {
                new MpDeposit() {},
                new MpDeposit() {},
                new MpDeposit() {},
                new MpDeposit() {},
                new MpDeposit() {},
                new MpDeposit() {},
                new MpDeposit() {},
                new MpDeposit() {},
                new MpDeposit() {},
                new MpDeposit() {},
                new MpDeposit() {},
            };

            _depositRepository.Setup(r => r.GetByName(It.IsAny<string>())).Returns(mpDeposits);

            // Act
            var result = _fixture.CreateDeposit(settlementEventDto);

            // Assert
            Assert.Equal($"{_pushpayWebEndpoint}/pushpay/0/settlements?includeCardSettlements=True&includeAchSettlements=True&fromDate=02-03-2018&toDate=02-03-2018", result.VendorDetailUrl);
            Assert.Equal(Decimal.Parse(amount), result.DepositTotalAmount);
            Assert.Equal("EFGHIJKLMNO" + "011", result.DepositName);
            Assert.True(14 >= result.DepositName.Length);
        }

        [Fact]
        public void ShouldSendDepositObjectToRepoLayer()
        {
            // Arrange
            var depositDto = new DepositDto
            {
                DepositName = "aaa",
                ProcessorTransferId = "aaa111",
                Id = 1234567
            };

            var newDepositDto = new DepositDto
            {
                DepositName = "aaa",
                ProcessorTransferId = "aaa111",
                Id = 1234567
            };

            var newMpDeposit = new MpDeposit
            {
                Id = 1234567
            };

            _mapper.Setup(m => m.Map<MpDeposit>(It.IsAny<DepositDto>())).Returns(newMpDeposit);
            _mapper.Setup(m => m.Map<DepositDto>(It.IsAny<MpDeposit>())).Returns(newDepositDto);
            _depositRepository.Setup(r => r.CreateDeposit(It.IsAny<MpDeposit>())).Returns(newMpDeposit);

            // Act
            var result = _fixture.SaveDeposit(depositDto);

            // Assert
            Assert.NotEqual(0, result.Id);
        }

        [Fact]
        public void ShouldGetDepositByProcessorTransferId()
        {
            // Arrange
            var processorTransferId = "111aaa222bbb";

            var depositDto = new DepositDto
            {
                ProcessorTransferId = processorTransferId
            };

            var mpDeposit = new MpDeposit
            {
                ProcessorTransferId = processorTransferId
            };

            _mapper.Setup(m => m.Map<MpDeposit>(It.IsAny<DepositDto>())).Returns(new MpDeposit());
            _mapper.Setup(m => m.Map<DepositDto>(It.IsAny<MpDeposit>())).Returns(depositDto);
            _depositRepository.Setup(m => m.GetDepositByProcessorTransferId(processorTransferId)).Returns(mpDeposit);

            // Act
            var result = _fixture.GetDepositByProcessorTransferId(processorTransferId);

            // Assert
            Assert.Equal(processorTransferId, result.ProcessorTransferId);
        }

        [Fact]
        public void ShouldGetDepositsToSync()
        {
            // Arrange
            var startDate = new DateTime(2017, 12, 6);
            var endDate = new DateTime(2017, 12, 13);

            var depositDtos = new List<SettlementEventDto>
            {
                new SettlementEventDto
                {
                    Key = "111bbb222aaa"
                }
            };

            _pushpayService.Setup(m => m.GetDepositsByDateRange(startDate, endDate)).Returns(depositDtos);
            _depositRepository.Setup(m => m.GetByTransferIds(It.IsAny<List<string>>()))
                .Returns(new List<MpDeposit>());

            // Act
            var result = _fixture.GetDepositsForSync(startDate, endDate);

            // Assert
            Assert.NotNull(result);
        }
    }
}
