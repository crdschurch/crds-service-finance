
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Services.Donations;
using MinistryPlatform.Donations;
using MinistryPlatform.Models;
using Moq;
using Xunit;

namespace Crossroads.Service.Finance.Test.Donations
{
    public class DonationServiceTest
    {
        private readonly Mock<IDonationRepository> _donationRepository;
        private readonly Mock<IMapper> _mapper;

        private readonly IDonationService _fixture;

        public DonationServiceTest()
        {
            _donationRepository = new Mock<IDonationRepository>();
            _mapper = new Mock<IMapper>();

            _fixture = new DonationService(_donationRepository.Object, _mapper.Object);
        }

        [Fact]
        public void ShouldGetDonationByTransactionCode()
        {
            // Arrange
            var transactionCode = "111aaa222bbb";

            var donationDto = new DonationDto
            {
                
            };

            var mpDonation = new MpDonation()
            {
                
            };

            _mapper.Setup(m => m.Map<MpDeposit>(It.IsAny<DepositDto>())).Returns(new MpDeposit());
            //_mapper.Setup(m => m.Map<DepositDto>(It.IsAny<MpDeposit>())).Returns(depositDto);
            //_depositRepository.Setup(m => m.GetDepositByProcessorTransferId(processorTransferId)).Returns(mpDeposit);

            // Act


            // Assert
        }

        [Fact]
        public void ShouldSetDonationStatus()
        {
            // Arrange


            // Act


            // Assert
        }

        [Fact]
        public void ShouldUpdateDonations()
        {
            // Arrange


            // Act


            // Assert
        }
    }
}
