
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services;
using MinistryPlatform.Interfaces;
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

            _mapper.Setup(m => m.Map<MpDonation>(It.IsAny<DonationDto>())).Returns(mpDonation);
            _mapper.Setup(m => m.Map<DonationDto>(It.IsAny<MpDonation>())).Returns(donationDto);
            _donationRepository.Setup(m => m.GetDonationByTransactionCode(It.IsAny<string>())).Returns(mpDonation);

            // Act
            var result = _fixture.GetDonationByTransactionCode(transactionCode);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldSetDonationStatus()
        {
            // Arrange
            var batchId = 1234567;

            var donations = new List<DonationDto>
            {
                new DonationDto
                {
                    DonationStatusId = DonationStatus.Pending.GetHashCode()
                }
            };

            // Act
            var result = _fixture.SetDonationStatus(donations, batchId);

            // Assert
            Assert.Equal(DonationStatus.Deposited.GetHashCode(), result[0].DonationStatusId);
        }

        [Fact]
        public void ShouldUpdateDonations()
        {
            // Arrange
            var mpDonations = new List<MpDonation>
            {

            };

            var donationDtos = new List<DonationDto>
            {

            };

            _mapper.Setup(m => m.Map<List<MpDonation>>(It.IsAny<List<DonationDto>>())).Returns(mpDonations);
            _mapper.Setup(m => m.Map<List<DonationDto>>(It.IsAny<List<MpDonation>>())).Returns(donationDtos);
            _donationRepository.Setup(m => m.UpdateDonations(It.IsAny<List<MpDonation>>())).Returns(mpDonations);

            // Act
            var result = _fixture.UpdateDonations(donationDtos);
            
            // Assert
            Assert.NotNull(result);
        }
    }
}
