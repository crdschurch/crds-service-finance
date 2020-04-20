using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services.Recurring
{
    public class RecurringService : IRecurringService
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IPushpayService _pushpayService;
        private readonly IConfigurationWrapper _configurationWrapper;

        private const int PausedRecurringGiftStatus = 2;

        public RecurringService(IDepositRepository depositRepository,
            IMapper mapper,
            IPushpayService pushpayService,
            IConfigurationWrapper configurationWrapper,
            IRecurringGiftRepository recurringGiftRepository)
        {
            _pushpayService = pushpayService;
            _configurationWrapper = configurationWrapper;
            _recurringGiftRepository = recurringGiftRepository;
        }

        public async Task<List<string>> SyncRecurringGifts(DateTime startDate, DateTime endDate)
        {
            var start = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var end = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            int numUpdates = 0;
            var giftIdsSynced = new List<string>();

            Console.WriteLine($"Starting SyncRecurringGifts at {DateTime.Now:G}");

            // get new and updated recurring gifts.
            var pushpayRecurringGifts = await _pushpayService.GetRecurringGiftsByDateRange(start, end);

            Console.WriteLine($"Syncing {pushpayRecurringGifts.Count} gifts from pushpay");

            // next, check to see if these gifts exist in MP
            var pushpayRecurringGiftIds = pushpayRecurringGifts
                .Select(r => r.PaymentToken).ToList();
            while (pushpayRecurringGiftIds.Any())
            {
                Thread.Sleep(500);

                // if the recurring gift does not exist in MP, pull the data from Pushpay and create it
                var range = Math.Min(pushpayRecurringGiftIds.Count, 1);

                var pushpayGiftIdsToSync = pushpayRecurringGiftIds.Take(range).ToList();
                pushpayRecurringGiftIds.RemoveRange(0, range);

                var mpRecurringGifts = await
                    _recurringGiftRepository.FindRecurringGiftsBySubscriptionIds(pushpayGiftIdsToSync);

                foreach (var pushpayRecurringGiftId in pushpayGiftIdsToSync)
                {
                    var mpGift = mpRecurringGifts.FirstOrDefault(r => r.SubscriptionId == pushpayRecurringGiftId);
                    var pushPayGift = pushpayRecurringGifts.First(r => r.PaymentToken == pushpayRecurringGiftId);

                    // if the recurring gift does not exist in MP, then create it
                    if (mpGift == null)
                    {
                        _logger.Info($"Create new recurring gift: {pushpayRecurringGiftId}");
                        Console.WriteLine($"Create new recurring gift: {pushpayRecurringGiftId}");
                        await _pushpayService.BuildAndCreateNewRecurringGift(pushPayGift, null);
                        giftIdsSynced.Add(pushpayRecurringGiftId);
                    }
                    // if the recurring gift DOES exist in MP, check to see when it was last updated and update it if the Pushpay version is newer
                    else
                    {
                        _logger.Info($"Comparing dates for {pushpayRecurringGiftId} ? MP: {mpGift.UpdatedOn.ToString()}, Pushpay: {pushPayGift.UpdatedOn.ToString()}");
                        Console.WriteLine($"Comparing dates for {pushpayRecurringGiftId} ? MP: {mpGift.UpdatedOn.ToString()}, Pushpay: {pushPayGift.UpdatedOn.ToString()}");
                        if (IsPushpayDateNewer(mpGift.UpdatedOn ?? DateTime.MinValue, pushPayGift.UpdatedOn))
                        {
                            _logger.Info($"Create new recurring gift: {pushpayRecurringGiftId}");
                            Console.WriteLine($"Update existing recurring gift: {pushpayRecurringGiftId}");
                            await _pushpayService.UpdateRecurringGiftForSync(pushPayGift, mpGift);
                            giftIdsSynced.Add(pushpayRecurringGiftId);
                        }
                    }
                }

                numUpdates += giftIdsSynced.Count;
            }

            _logger.Info($"{numUpdates} Recurring gifts synced from Pushpay: {string.Join(", ", giftIdsSynced)}");
            Console.WriteLine($"{numUpdates} Recurring gifts synced from Pushpay: {string.Join(", ", giftIdsSynced)}");
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
