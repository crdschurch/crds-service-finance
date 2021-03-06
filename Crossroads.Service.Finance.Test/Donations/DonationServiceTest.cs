﻿using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Services;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Mock;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Crossroads.Service.Finance.Test.Donations
{
    public class DonationServiceTest
    {
        private readonly Mock<IDonationRepository> _donationRepository;
        private readonly Mock<IDonationDistributionRepository> _donationDistributionRepository;
        private readonly Mock<IPledgeRepository> _pledgeRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IContactService> _contactService;

        private readonly IDonationService _fixture;

        public DonationServiceTest()
        {
            _donationRepository = new Mock<IDonationRepository>();
            _donationDistributionRepository = new Mock<IDonationDistributionRepository>();
            _pledgeRepository = new Mock<IPledgeRepository>();
            _mapper = new Mock<IMapper>();
            _contactService = new Mock<IContactService>();

            _fixture = new DonationService(_donationRepository.Object, _donationDistributionRepository.Object, _pledgeRepository.Object, _mapper.Object, _contactService.Object);
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
            _donationRepository.Setup(m => m.GetDonationByTransactionCode(It.IsAny<string>())).Returns(Task.FromResult(mpDonation));

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
            _donationRepository.Setup(m => m.Update(It.IsAny<List<MpDonation>>())).Returns(Task.FromResult(mpDonations));

            // Act
            var result = _fixture.Update(donationDtos);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldCreateAndReturnRecurringGiftObject()
        {
            // Arrange
            var contactId = 5544555;

            var recurringGiftDto = new List<RecurringGiftDto>
            {
                new RecurringGiftDto {
                    RecurringGiftId = 1,
                    ContactId = 123,
                    DonorId = 456,
                    FrequencyId = 1,
                    DayOfMonth = 15,
                    DayOfWeek = 1,
                    Amount = 25,
                    ProgramId = 1,
                    SubscriptionId = "123",
                    SourceUrl = "localhost",
                    PredefinedAmount = 50,
                    VendorDetailUrl = "localhost"
                }
            };

            _mapper.Setup(m => m.Map<List<RecurringGiftDto>>(It.IsAny<List<MpRecurringGift>>())).Returns(recurringGiftDto);
            _donationRepository.Setup(r => r.GetRecurringGifts(It.IsAny<int>())).Returns(Task.FromResult(MpRecurringGiftMock.CreateList(123)));

            // Act
            var result = _fixture.GetRecurringGifts(contactId).Result;

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void ShouldCalculatePledges()
        {
            // Arrange
            var token = "123abc";
            var contactId = 1234567;
            var pledgeIds = new int[] { 12, 25, 66 };

            _contactService.Setup(m => m.GetContactIdBySessionId(token)).Returns(Task.FromResult(contactId));
            _pledgeRepository.Setup(r => r.GetActiveAndCompleted(It.IsAny<int>())).Returns(Task.FromResult(MpPledgeMock.CreateList(pledgeIds[0], pledgeIds[1], pledgeIds[2])));
            _donationDistributionRepository.Setup(r => r.GetByPledges(It.IsAny<List<int>>())).Returns(Task.FromResult(MpDonationDistributionMock.CreateList(pledgeIds[0], pledgeIds[1])));

            // Act
            var result = _fixture.CalculatePledges(contactId).Result;

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
            // Arrange
            var donationDto = new List<DonationDto>
            {
                new DonationDto {
                DonationId = 1,
                DonationAmt = 25,
                DonationStatusId = 1,
                BatchId = 456,
                TransactionCode = "Test"
                }
            };

            var donationHistory = new List<DonationDetailDto>
            {
                new DonationDetailDto
                {
                    DonationId = 5544555
                }
            };

            _mapper.Setup(m => m.Map<List<DonationDto>>(It.IsAny<List<MpDonation>>())).Returns(donationDto);
            _donationRepository.Setup(r => r.GetDonationHistoryByContactId(It.IsAny<int>(), null, null)).Returns(Task.FromResult(MpDonationDetailMock.CreateList()));
            _mapper.Setup(m => m.Map<List<DonationDetailDto>>(It.IsAny<List<MpDonationDetail>>())).Returns(donationHistory);

            // Act
            var result = _fixture.GetDonations("token").Result;

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void ShouldGetDonationHistoryObjects()
        {
            // Arrange
            var contactId = 1234567;
            var token = "123abc";

            var donationHistory = new List<DonationDetailDto>
            {
                new DonationDetailDto
                {
                    DonationId = 5544555
                }
            };

            var mpDonationHistories = new List<MpDonationDetail>
            {
                new MpDonationDetail
                {
                    DonationId = 5544555
                }
            };

            var contacts = new List<ContactDto>
            {
                new ContactDto
                {
                    ContactId = 1234567
                }
            };

            _contactService.Setup(m => m.GetContactIdBySessionId(token)).Returns(Task.FromResult(contactId));
            _contactService.Setup(m => m.GetCogiversByContactId(contactId)).Returns(Task.FromResult(contacts));
            _mapper.Setup(m => m.Map<List<DonationDetailDto>>(It.IsAny<List<MpDonationDetail>>())).Returns(donationHistory);
            _donationRepository.Setup(m => m.GetDonationHistoryByContactId(contactId, null, null)).Returns(Task.FromResult(mpDonationHistories));

            // Act
            var result = _fixture.GetDonations(contactId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5544555, result[0].DonationId);
        }
        
        [Fact]
        public async Task GetDonorAccounts()
        {

            // Arrange
            var donorId = 777790;
            var mpDonorAccount = new MpDonorAccount
            {
                Closed = false,
                AccountNumber = "4894894894",
                InstitutionName = "Bank Of America",
                DomainId = 1,
                DonorId = 777790,
                DonorAccountId = 8098
            };

            _donationRepository.Setup(m => m.GetDonorAccounts(donorId))
                .ReturnsAsync(new List<MpDonorAccount> {mpDonorAccount});
            
            
            // Act
            var results = await _fixture.GetDonorAccounts(donorId);

            // Assert
            Assert.Contains(mpDonorAccount, results);
        }

        [Fact]
        public void ShouldGetDonationsByTransactionCodes()
        {
            // Arrange
            var transactionCode = "111aaa222bbb";

            var transactionCodes = new List<string>
            {
                "111aaa222bbb",
                "333ccc444ddd"
            };

            var mpDonationList = new List<MpDonation>
            {
                new MpDonation()
            };

            var donationList = new List<DonationDto>
            {
                new DonationDto()
            };

            _mapper.Setup(m => m.Map<List<MpDonation>>(It.IsAny<List<DonationDto>>())).Returns(mpDonationList);
            _mapper.Setup(m => m.Map<List<DonationDto>>(It.IsAny<List<MpDonation>>())).Returns(donationList);

            _donationRepository.Setup(m => m.GetDonationsByTransactionIds(It.IsAny<List<string>>())).Returns(Task.FromResult(mpDonationList));

            // Act
            var result = _fixture.GetDonationsByTransactionCodes(transactionCodes);

            // Assert
            Assert.NotNull(result);
        }
    }
}
