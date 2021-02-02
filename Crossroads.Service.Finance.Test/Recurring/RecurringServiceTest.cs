using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Services.Recurring;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Moq;
using Pushpay.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Services.Donor;
using Xunit;

namespace Crossroads.Service.Finance.Test.Recurring
{
    public class RecurringServiceTest
    {
        private readonly Mock<IDepositRepository> _depositRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IPushpayService> _pushpayService;
        private readonly Mock<IConfigurationWrapper> _configWrapper;
        private readonly Mock<IRecurringGiftRepository> _recurringGiftRepository;
        private readonly Mock<INewPushpayService> _newPushpayService;
        private readonly Mock<IDonationService> _donationService;
        private readonly Mock<IProgramRepository> _programRepository;
        private readonly Mock<IDonorService> _donorService;
        private readonly Mock<IGatewayService> _gatewayService;

        private readonly IRecurringService _fixture;

        public RecurringServiceTest()
        {
            _depositRepository = new Mock<IDepositRepository>();
            _mapper = new Mock<IMapper>();
            _pushpayService = new Mock<IPushpayService>();
            _configWrapper = new Mock<IConfigurationWrapper>();
            _recurringGiftRepository = new Mock<IRecurringGiftRepository>();
            _newPushpayService = new Mock<INewPushpayService>();
            _donationService = new Mock<IDonationService>();
            _programRepository = new Mock<IProgramRepository>();
            _donorService = new Mock<IDonorService>();
            _gatewayService = new Mock<IGatewayService>();

            _fixture = new RecurringService(_depositRepository.Object, _mapper.Object, _pushpayService.Object, _newPushpayService.Object, _configWrapper.Object,
               _donationService.Object, _donorService.Object, _recurringGiftRepository.Object, _programRepository.Object, _gatewayService.Object);
        }

        [Fact]
        public void ShouldSyncRecurringGifts()
        {
            // Arrange
            var pushpayRecurringGifts = new List<PushpayRecurringGiftDto>
            {
                new PushpayRecurringGiftDto
                {
                    PaymentToken = "123abc"
                }
            };

            _pushpayService.Setup(m => m.GetRecurringGiftsByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(pushpayRecurringGifts);

            _recurringGiftRepository.Setup(m => m.FindRecurringGiftsBySubscriptionIds(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(new List<MpRecurringGift>()));

            _pushpayService.Setup(m => m.BuildAndCreateNewRecurringGift(It.IsAny<PushpayRecurringGiftDto>(), It.IsAny<int?>()))
                .Returns(Task.FromResult(new MpRecurringGift{ SubscriptionId = "123abc"}));

            // Act
            var result = _fixture.SyncRecurringGifts(DateTime.Now, DateTime.Now.AddDays(1)).Result;

            // Assert
            _pushpayService.VerifyAll();
            _recurringGiftRepository.VerifyAll();

            Assert.NotEmpty(result);
        }

        [Fact]
        public void ShouldNotSyncRecurringGiftsIfBothGiftsArePaused()
        {
            // Arrange
            var pushpayRecurringGifts = new List<PushpayRecurringGiftDto>
            {
                new PushpayRecurringGiftDto
                {
                    PaymentToken = "123abc",
                    Status = "Paused"
                }
            };

            _pushpayService.Setup(m => m.GetRecurringGiftsByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(pushpayRecurringGifts);

            _recurringGiftRepository.Setup(m => m.FindRecurringGiftsBySubscriptionIds(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(new List<MpRecurringGift>{ new MpRecurringGift {SubscriptionId = "123abc" }}));

            // Act
            var result = _fixture.SyncRecurringGifts(DateTime.Now, DateTime.Now.AddDays(1)).Result;

            // Assert
            _pushpayService.VerifyAll();
            _recurringGiftRepository.VerifyAll();

            Assert.Empty(result);
        }

        [Fact]
        public void ShouldUpdateRecurringGiftsWithOlderMpGift()
        {
            // Arrange
            var pushpayRecurringGifts = new List<PushpayRecurringGiftDto>
            {
                new PushpayRecurringGiftDto
                {
                    PaymentToken = "123abc",
                    UpdatedOn = new DateTime(2019, 05, 23)
                }
            };

            var mpRecurringGifts = new List<MpRecurringGift>
            {
                new MpRecurringGift
                {
                    SubscriptionId = "123abc",
                    UpdatedOn = new DateTime(2019, 05, 22)
                }
            };

            _pushpayService.Setup(m => m.GetRecurringGiftsByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(pushpayRecurringGifts);

            _recurringGiftRepository.Setup(m => m.FindRecurringGiftsBySubscriptionIds(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(mpRecurringGifts));

            _pushpayService.Setup(m => m.UpdateRecurringGiftForSync(pushpayRecurringGifts[0], mpRecurringGifts[0]))
                .Returns(Task.FromResult(new RecurringGiftDto()));

            // Act
            var result = _fixture.SyncRecurringGifts(DateTime.Now, DateTime.Now.AddDays(1)).Result;

            // Assert
            _pushpayService.VerifyAll();
            _recurringGiftRepository.VerifyAll();

            Assert.Single(result);
        }

        [Fact]
        public void ShouldSyncRecurringGiftsIfPushpayGiftIsPausedAndMpWasUpdatedEarlierInTheDay()
        {
            // Arrange
            var pushpayRecurringGifts = new List<PushpayRecurringGiftDto>
            {
                new PushpayRecurringGiftDto
                {
                    PaymentToken = "123abc",
                    UpdatedOn = DateTime.Now,
                    Status = "Paused"
                }
            };

            var mpRecurringGifts = new List<MpRecurringGift>
            {
                new MpRecurringGift
                {
                    SubscriptionId = "123abc",
                    UpdatedOn = DateTime.Now.AddMinutes(-5)
                }
            };

            _pushpayService.Setup(m => m.GetRecurringGiftsByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(pushpayRecurringGifts);

            _recurringGiftRepository.Setup(m => m.FindRecurringGiftsBySubscriptionIds(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(mpRecurringGifts));

            _pushpayService.Setup(m => m.UpdateRecurringGiftForSync(pushpayRecurringGifts[0], mpRecurringGifts[0]))
                .Returns(Task.FromResult(new RecurringGiftDto()));

            // Act
            var result = _fixture.SyncRecurringGifts(DateTime.Now, DateTime.Now.AddDays(1)).Result;

            // Assert
            _pushpayService.VerifyAll();
            _recurringGiftRepository.VerifyAll();

            Assert.Single(result);
        }

        [Fact]
        public void ShouldSyncRecurringGiftsIfMpGiftIsPausedAndPushpayWasNot()
        {
            // Arrange
            var testDate = DateTime.Parse(DateTime.Now.ToShortDateString());

            var pushpayRecurringGifts = new List<PushpayRecurringGiftDto>
            {
                new PushpayRecurringGiftDto
                {
                    PaymentToken = "123abc",
                    UpdatedOn = DateTime.Now
                }
            };

            var mpRecurringGifts = new List<MpRecurringGift>
            {
                new MpRecurringGift
                {
                    SubscriptionId = "123abc",
                    UpdatedOn = DateTime.Now.AddMinutes(-5),
                    Status = "Paused"
                }
            };

            _pushpayService.Setup(m => m.GetRecurringGiftsByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(pushpayRecurringGifts);

            _recurringGiftRepository.Setup(m => m.FindRecurringGiftsBySubscriptionIds(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(mpRecurringGifts));

            _pushpayService.Setup(m => m.UpdateRecurringGiftForSync(pushpayRecurringGifts[0], mpRecurringGifts[0]))
                .Returns(Task.FromResult(new RecurringGiftDto()));

            // Act
            var result = _fixture.SyncRecurringGifts(testDate.AddDays(-1), testDate).Result;

            // Assert
            _pushpayService.VerifyAll();
            _recurringGiftRepository.VerifyAll();

            Assert.Single(result);
        }

        [Fact]
        public void ShouldSetRecurringGiftStatusChangedDate()
        {
            // Arrange
            var pushpayRecurringGift = new PushpayRecurringGiftDto
            {
	            Status = "Paused",
	            Amount = null,
	            Payer = null,
	            PaymentMethodType = null,
	            Card = null,
	            Account = null,
	            Fund = null,
	            PaymentToken = null,
	            Links = new PushpayLinksDto(),
	            UpdatedOn = default,
	            PushpayFields = null,
	            Campus = new PushpayCampusDto
	            {
		            Name = "Oakley",
		            Key = "Oakley"
	            },
	            Schedule = null,
	            Notes = null,
	            ExternalLinks = new ExternalLinks[]
	            {
	            }
            };

            var mpRecurringGift = new MpRecurringGift
            {
	            RecurringGiftId = 0,
	            ContactId = 0,
	            DonorId = 0,
	            DonorAccountId = 9988776,
	            FrequencyId = 0,
	            DayOfMonth = null,
	            DayOfWeek = null,
	            Amount = 0,
	            StartDate = default,
	            EndDate = null,
	            ProgramId = 550,
	            ProgramName = null,
	            CongregationId = 0,
	            SubscriptionId = null,
	            ConsecutiveFailureCount = 0,
	            SourceUrl = null,
	            PredefinedAmount = null,
	            VendorDetailUrl = null,
	            VendorAdminDetailUrl = null,
	            Notes = null,
	            Status = "Active",
	            RecurringGiftStatusId = 1,
	            UpdatedOn = null,
	            StatusChangedDate = new DateTime(2021, 2, 1)
            };

            int? donorId = 5544555;

            var mpDonorAccount = new MpDonorAccount
            {
	            DonorAccountId = 0,
	            DonorId = 0,
	            NonAssignable = false,
	            DomainId = 0,
	            AccountTypeId = 0,
	            Closed = false,
	            InstitutionName = null,
	            AccountNumber = null,
	            RoutingNumber = null,
	            ProcessorId = null,
	            ProcessorTypeId = null
            };

            var congregationId = 1;
            var fundId = 550;
            var pushpayRecurringGiftStatus = "Paused";

            _mapper.Setup(m => m.Map<MpRecurringGift>(It.IsAny<PushpayRecurringGiftDto>())).Returns(mpRecurringGift);
            _donorService.Setup(m => m.FindDonorId(pushpayRecurringGift)).Returns(Task.FromResult(donorId));

            // this gets called because the donor id has a value
            _donationService.Setup(m => m.FindDonorAccount(pushpayRecurringGift, donorId.GetValueOrDefault()))
	            .Returns(Task.FromResult(mpDonorAccount));

            _pushpayService.Setup(m => m.LookupCongregationId(pushpayRecurringGift.PushpayFields, pushpayRecurringGift.Campus.Key))
	            .Returns(Task.FromResult(congregationId));

            _newPushpayService.Setup(m => m.ParseFundIdFromExternalLinks(pushpayRecurringGift))
	            .Returns(fundId);

            _pushpayService.Setup(m => m.GetRecurringGiftStatusId(pushpayRecurringGift.Status)).Returns(3);

            _pushpayService.Setup(m => m.GetRecurringGiftNotes(pushpayRecurringGift)).Returns("blah blah");

	            // Act
            var result = _fixture.BuildRecurringScheduleFromPushPayData(pushpayRecurringGift).Result;

            // Assert
            Assert.Equal(new DateTime(2021, 2, 1).ToShortDateString(), mpRecurringGift.StatusChangedDate.GetValueOrDefault().ToShortDateString());
        }

        [Fact]
        public void ShouldNotSetRecurringGiftStatusChangedDate()
        {
            // Arrange
            var pushpayRecurringGift = new PushpayRecurringGiftDto
            {
                Status = "Active",
                Amount = null,
                Payer = null,
                PaymentMethodType = null,
                Card = null,
                Account = null,
                Fund = null,
                PaymentToken = null,
                Links = new PushpayLinksDto(),
                UpdatedOn = new DateTime(2021, 2, 1),
                PushpayFields = null,
                Campus = new PushpayCampusDto
                {
                    Name = "Oakley",
                    Key = "Oakley"
                },
                Schedule = null,
                Notes = null,
                ExternalLinks = new ExternalLinks[]
                {
                }
            };

            var mpRecurringGift = new MpRecurringGift
            {
                RecurringGiftId = 0,
                ContactId = 0,
                DonorId = 0,
                DonorAccountId = 9988776,
                FrequencyId = 0,
                DayOfMonth = null,
                DayOfWeek = null,
                Amount = 0,
                StartDate = default,
                EndDate = null,
                ProgramId = 550,
                ProgramName = null,
                CongregationId = 0,
                SubscriptionId = null,
                ConsecutiveFailureCount = 0,
                SourceUrl = null,
                PredefinedAmount = null,
                VendorDetailUrl = null,
                VendorAdminDetailUrl = null,
                Notes = null,
                Status = "Active",
                RecurringGiftStatusId = 1,
                UpdatedOn = null,
                StatusChangedDate = new DateTime(2021, 1, 1)
            };

            int? donorId = 5544555;

            var mpDonorAccount = new MpDonorAccount
            {
                DonorAccountId = 0,
                DonorId = 0,
                NonAssignable = false,
                DomainId = 0,
                AccountTypeId = 0,
                Closed = false,
                InstitutionName = null,
                AccountNumber = null,
                RoutingNumber = null,
                ProcessorId = null,
                ProcessorTypeId = null
            };

            var congregationId = 1;
            var fundId = 550;
            var pushpayRecurringGiftStatus = "Paused";

            _mapper.Setup(m => m.Map<MpRecurringGift>(It.IsAny<PushpayRecurringGiftDto>())).Returns(mpRecurringGift);
            _donorService.Setup(m => m.FindDonorId(pushpayRecurringGift)).Returns(Task.FromResult(donorId));

            // this gets called because the donor id has a value
            _donationService.Setup(m => m.FindDonorAccount(pushpayRecurringGift, donorId.GetValueOrDefault()))
                .Returns(Task.FromResult(mpDonorAccount));

            _pushpayService.Setup(m => m.LookupCongregationId(pushpayRecurringGift.PushpayFields, pushpayRecurringGift.Campus.Key))
                .Returns(Task.FromResult(congregationId));

            _newPushpayService.Setup(m => m.ParseFundIdFromExternalLinks(pushpayRecurringGift))
                .Returns(fundId);

            _pushpayService.Setup(m => m.GetRecurringGiftStatusId(pushpayRecurringGift.Status)).Returns(1);

            _pushpayService.Setup(m => m.GetRecurringGiftNotes(pushpayRecurringGift)).Returns("blah blah");

            // Act
            var result = _fixture.BuildRecurringScheduleFromPushPayData(pushpayRecurringGift).Result;

            // Assert
            Assert.Equal(new DateTime(2021, 1, 1), mpRecurringGift.StatusChangedDate);
        }

    }
}
