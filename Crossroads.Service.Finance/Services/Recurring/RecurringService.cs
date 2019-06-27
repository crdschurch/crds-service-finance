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

        private const int PausedRecurringGiftStatus = 2;

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

            int numUpdates = 0;
            var giftIdsSynced = new List<string>();

            Console.WriteLine($"Starting SyncRecurringGifts at {DateTime.Now:G}");

            // get new and updated recurring gifts.
            var pushpayRecurringGifts = _pushpayService.GetRecurringGiftsByDateRange(start, end);

            Console.WriteLine($"Syncing {pushpayRecurringGifts.Count} gifts from pushpay");

            // next, check to see if these gifts exist in MP
            var pushpayRecurringGiftIds = pushpayRecurringGifts
                .Select(r => r.PaymentToken).ToList();
            while (pushpayRecurringGiftIds.Any())
            {
                // if the recurring gift does not exist in MP, pull the data from Pushpay and create it
                var range = Math.Min(pushpayRecurringGiftIds.Count, 25);

                var pushpayGiftIdsToSync = pushpayRecurringGiftIds.Take(range).ToList();
                pushpayRecurringGiftIds.RemoveRange(0, range);

                var mpRecurringGifts =
                    _recurringGiftRepository.FindRecurringGiftsBySubscriptionIds(pushpayGiftIdsToSync);

                foreach (var pushpayRecurringGiftId in pushpayGiftIdsToSync)
                {
                    var mpGift = mpRecurringGifts.FirstOrDefault(r => r.SubscriptionId == pushpayRecurringGiftId);
                    var pushPayGift = pushpayRecurringGifts.First(r => r.PaymentToken == pushpayRecurringGiftId);

                    // if the recurring gift does not exist in MP, then create it
                    if (mpGift == null)
                    {
                        Console.WriteLine($"create new {pushpayRecurringGiftId}");
                        _pushpayService.BuildAndCreateNewRecurringGift(pushPayGift);
                        giftIdsSynced.Add(pushpayRecurringGiftId);
                    }
                    // if the recurring gift DOES exist in MP, check to see when it was last updated and update it if the Pushpay version is newer
                    else
                    {
                        Console.WriteLine($"comparing dates for {pushpayRecurringGiftId} ? mp: {mpGift.UpdatedOn.ToString()}, pushpay: {pushPayGift.UpdatedOn.ToString()}");
                        if (IsPushpayDateNewer(mpGift.UpdatedOn ?? DateTime.MinValue, pushPayGift.UpdatedOn))
                        {
                            Console.WriteLine($"update existing {pushpayRecurringGiftId}");
                            _pushpayService.UpdateRecurringGiftForSync(pushPayGift, mpGift);
                            giftIdsSynced.Add(pushpayRecurringGiftId);
                        }
                    }
                }

                // last, log the subscription ids of the gifts that were updated
                var syncedRecurringGiftsEntry = new LogEventEntry(LogEventType.syncedRecurringGifts);
                syncedRecurringGiftsEntry.Push("recurringGiftsSyncedFromPushpay", string.Join(",", giftIdsSynced));
                _dataLoggingService.LogDataEvent(syncedRecurringGiftsEntry);

                numUpdates += giftIdsSynced.Count;
            }

            Console.WriteLine($"Finished SyncRecurringGifts at {DateTime.Now:G}.  {numUpdates} records updated.");
            return giftIdsSynced;
        }
        
        private bool IsPushpayDateNewer(DateTime mp, DateTime pushpay)
        {
            // MP truncates seconds from DateTime fields that are inserted/updated via
            // the MP REST API, so exclude seconds from our comparison to prevent false
            // positives.
            DateTime normalizedMp = new DateTime(mp.Year, mp.Month, mp.Day, mp.Hour, mp.Minute, 0);
            DateTime normalizedPushpay = new DateTime(pushpay.Year, pushpay.Month, pushpay.Day, pushpay.Hour, pushpay.Minute, 0);

            return normalizedPushpay > normalizedMp;
        }
    }
}
