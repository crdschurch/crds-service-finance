using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Xunit;
using Moq;

namespace Crossroads.Service.Finance.Test.Deposits
{
    public class DepositServiceTest
    {
        private readonly Mock<IDepositRepository> _depositRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IPushpayService> _pushpayService;

        private readonly IDepositService _fixture;

        public DepositServiceTest()
        {
            _depositRepository = new Mock<IDepositRepository>();
            _mapper = new Mock<IMapper>();
            _pushpayService = new Mock<IPushpayService>();

            _fixture = new DepositService(_depositRepository.Object, _mapper.Object, _pushpayService.Object);
        }

        [Fact]
        public void ShouldCreateAndReturnDepositObject()
        {
            // Arrange
            var depositName = "testDepositName";
            var settlementKey = "aaabbb111222";
            var amount = "1000";

            var settlementEventDto = new SettlementEventDto
            {
                Key = settlementKey,
                TotalAmount = new AmountDto
                {
                    Amount = amount
                }
            };

            // Act
            var result = _fixture.CreateDeposit(settlementEventDto, depositName);

            // Assert
            Assert.Equal(settlementKey, result.ProcessorTransferId);
            Assert.Equal(Decimal.Parse(amount), result.DepositTotalAmount);
            Assert.Equal(result.DepositName, depositName);
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

            var depositDtos = new List<DepositDto>
            {
                new DepositDto()
            };

            _pushpayService.Setup(m => m.GetDepositsByDateRange(startDate, endDate)).Returns(depositDtos);

            // Act
            var result = _fixture.GetDepositsForSync(startDate, endDate);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldPullExistingDepositsByTransferIdsFromDepositRepo()
        {
            // Arrange
            var transferIds = new List<string>
            {
                "111aaa222bbb",
                "333ddd444ccc"
            };

            _mapper.Setup(m => m.Map<List<DepositDto>>(It.IsAny<List<MpDeposit>>())).Returns(new List<DepositDto>());
            _depositRepository.Setup(m => m.GetDepositsByTransferIds(transferIds)).Returns(new List<MpDeposit>());

            // Act
            var result = _fixture.GetDepositsByTransferIds(transferIds);

            // Assert
            Assert.NotNull(result);
        }
    }
}
