﻿using AutoMapper;
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

    }
}
