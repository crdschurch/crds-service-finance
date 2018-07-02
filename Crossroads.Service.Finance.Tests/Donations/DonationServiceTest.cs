
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
using Mock;

namespace Crossroads.Service.Finance.Test.Donations
{
    public class DonationServiceTest
    {
        private readonly Mock<IDonationRepository> _donationRepository;
        private readonly Mock<IDonationDistributionRepository> _donationDistributionRepository;
        private readonly Mock<IPledgeRepository> _pledgeRepository;
        private readonly Mock<IMapper> _mapper;

        private readonly IDonationService _fixture;

        public DonationServiceTest()
        {
            _donationRepository = new Mock<IDonationRepository>();
            _donationDistributionRepository = new Mock<IDonationDistributionRepository>();
            _pledgeRepository = new Mock<IPledgeRepository>();
            _mapper = new Mock<IMapper>();

            _fixture = new DonationService(_donationRepository.Object, _donationDistributionRepository.Object, _pledgeRepository.Object, _mapper.Object);
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
            _donationRepository.Setup(m => m.Update(It.IsAny<List<MpDonation>>())).Returns(mpDonations);

            // Act
            var result = _fixture.Update(donationDtos);
            
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldCreateAndReturnRecurringGiftObject()
        {
            // Arrange
            var mpRecurringGifts = new List<MpRecurringGift>
            {
                new MpRecurringGift {
                RecurringGiftId = 1,
                ContactId = 123,
                DonorId = 456,
                DonorAccountId = 789,
                FrequencyId = 1,
                DayOfMonth = 15,
                DayOfWeek = 1,
                Amount = 50,
                StartDate = Convert.ToDateTime("2018-06-01"),
                ProgramId = 1,
                CongregationId = 1,
                SubscriptionId = "Test",
                ConsecutiveFailureCount = 0,
                SourceUrl = "localhost",
                PredefinedAmount = 100,
                VendorDetailUrl = "localhost"
                }
            };

            _mapper.Setup(m => m.Map<List<MpRecurringGift>>(It.IsAny<List<RecurringGiftDto>>())).Returns(mpRecurringGifts);
            _donationRepository.Setup(r => r.GetRecurringGifts(It.IsAny<int>())).Returns(mpRecurringGifts);

            // Act
            var result = _fixture.GetRecurringGifts("token");

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void ShouldCalculatePledges()
        {
            var pledgeIds = new int[] { 12, 25, 66 };
            _pledgeRepository.Setup(r => r.GetActiveAndCompleted(It.IsAny<int>())).Returns(MpPledgeMock.CreateList(pledgeIds[0], pledgeIds[1], pledgeIds[2]));
            _donationDistributionRepository.Setup(r => r.GetByPledges(It.IsAny<List<int>>())).Returns(MpDonationDistributionMock.CreateList(pledgeIds[0], pledgeIds[1]));

            // Act
            var result = _fixture.CalculatePledges("token");

            Console.WriteLine("result!");
            Console.WriteLine(result.Count);

            // Assert
            Assert.Equal(12, result[0].PledgeId);
            Assert.Equal(1101.12m, result[0].PledgeDonationsTotal);
            Assert.Equal(25, result[1].PledgeId);
            Assert.Equal(62.10m, result[1].PledgeDonationsTotal);
            Assert.Equal(66, result[2].PledgeId);
            Assert.Equal(0, result[2].PledgeDonationsTotal);
        }

        [Fact]
        public void ShouldCreateAndReturnDonationObject()
        {
        }
    }
}
