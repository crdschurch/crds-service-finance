using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Services.Deposits;
using MinistryPlatform.Deposits;
using MinistryPlatform.Models;
using Xunit;
using Moq;

namespace Crossroads.Service.Finance.Test.Deposits
{
    public class DepositServiceTest
    {
        private readonly Mock<IDepositRepository> _depositRepository;
        private readonly Mock<IMapper> _mapper;

        private readonly IDepositService _fixture;

        public DepositServiceTest()
        {
            _depositRepository = new Mock<IDepositRepository>();
            _mapper = new Mock<IMapper>();

            _fixture = new DepositService(_depositRepository.Object, _mapper.Object);
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

    }
}
