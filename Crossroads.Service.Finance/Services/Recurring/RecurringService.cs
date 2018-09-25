using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using Utilities.Logging;

namespace Crossroads.Service.Finance.Services.Recurring
{
    public class RecurringService : IRecurringService
    {
        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IPushpayService _pushpayService;
        private readonly IConfigurationWrapper _configurationWrapper;
        private readonly IDataLoggingService _dataLoggingService;

        public RecurringService(IDepositRepository depositRepository,
            IMapper mapper,
            IPushpayService pushpayService,
            IConfigurationWrapper configurationWrapper,
            IDataLoggingService dataLoggingService,
            IRecurringGiftRepository recurringGiftRepository)
        {
            _pushpayService = pushpayService;
            _configurationWrapper = configurationWrapper;
            _dataLoggingService = dataLoggingService;
            _recurringGiftRepository = recurringGiftRepository;
        }

        public List<string> SyncRecurringGifts(DateTime startDate, DateTime endDate)
        {
            var start = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var end = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            // get new and updated recurring gifts
            var pushpayRecurringGifts = _pushpayService.GetRecurringGiftsByDateRange(start, end);

            if (!pushpayRecurringGifts.Any())
            {
                return null;
            }

            // next, check to see if these gifts exist in MP
            var recurringGiftIds = pushpayRecurringGifts
                .Select(r => r.PaymentToken).ToList();

            var mpRecurringGifts =
                _recurringGiftRepository.FindRecurringGiftsBySubscriptionIds(recurringGiftIds);

            // if the recurring gift does not exist in MP, pull the data from Pushpay and create it
            var giftIdsSynced = new List<string>();

            foreach (var item in recurringGiftIds)
            {
                if (mpRecurringGifts.All(r => r.SubscriptionId != item))
                {
                    _pushpayService.BuildAndCreateNewRecurringGift(pushpayRecurringGifts.First(r => r.PaymentToken == item));
                    giftIdsSynced.Add(item);
                }
            }

            // if the recurring gift DOES exist in MP, check to see when it was last updated and update it if the Pushpay version is newer


            // last, log the subscription ids of the gifts that were updated
            // TODO: Refactor this
            var syncedRecurringGiftsEntry = new LogEventEntry(LogEventType.syncedRecurringGifts);
            syncedRecurringGiftsEntry.Push("New Recurring Gifts Synced from Pushpay", string.Join(",", giftIdsSynced));
            _dataLoggingService.LogDataEvent(syncedRecurringGiftsEntry);

            return giftIdsSynced;
        }
    }
}
