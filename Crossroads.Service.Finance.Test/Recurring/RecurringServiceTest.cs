using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services.Recurring;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Moq;
using Pushpay.Models;
using Utilities.Logging;
using Xunit;

namespace Crossroads.Service.Finance.Test.Recurring
{
    public class RecurringServiceTest
    {
        private readonly Mock<IDepositRepository> _depositRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IPushpayService> _pushpayService;
        private readonly Mock<IConfigurationWrapper> _configWrapper;
        private readonly Mock<IDataLoggingService> _dataLoggingService;
        private readonly Mock<IRecurringGiftRepository> _recurringGiftRepository;

        private readonly IRecurringService _fixture;

        public RecurringServiceTest()
        {
            _depositRepository = new Mock<IDepositRepository>();
            _mapper = new Mock<IMapper>();
            _pushpayService = new Mock<IPushpayService>();
            _configWrapper = new Mock<IConfigurationWrapper>();
            _dataLoggingService = new Mock<IDataLoggingService>();
            _recurringGiftRepository = new Mock<IRecurringGiftRepository>();

            _fixture = new RecurringService(_depositRepository.Object, _mapper.Object, _pushpayService.Object, _configWrapper.Object,
                _dataLoggingService.Object, _recurringGiftRepository.Object);
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
                .Returns(new List<MpRecurringGift>());

            _pushpayService.Setup(m => m.BuildAndCreateNewRecurringGift(It.IsAny<PushpayRecurringGiftDto>()))
                .Returns(new MpRecurringGift{ SubscriptionId = "123abc"});

            // Act
            var result = _fixture.SyncRecurringGifts(DateTime.Now, DateTime.Now.AddDays(1));
        }
    }
}
