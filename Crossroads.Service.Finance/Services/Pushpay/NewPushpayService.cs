using Crossroads.Service.Finance.Interfaces;
using MinistryPlatform.Interfaces;
using NLog;
using Pushpay.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Services
{
    //TODO: Rename this service and it's interface once the original files have been decommissioned
    public class NewPushpayService : INewPushpayService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPushpayClient _pushpayClient;
        private readonly IRecurringGiftRepository _recurringGiftRepository;

        public NewPushpayService(IPushpayClient pushpayClient, IRecurringGiftRepository recurringGiftRepository)
        {
            _pushpayClient = pushpayClient;
            _recurringGiftRepository = recurringGiftRepository;
        }

        public async Task PullRecurringGiftsAsync(DateTime startDate, DateTime endDate)
        {
            _logger.Info($"PullRecurringGiftsAsync is starting.  Start Date: {startDate}, End Date: {endDate}");
            var recurringGifts = await _pushpayClient.GetRecurringGiftsAsync(startDate, endDate);
            _logger.Info($"Got {recurringGifts.Count} updates to recurring gift schedules and/or new schedules from PushPay.");
            foreach (var recurringGift in recurringGifts)
            {
                _recurringGiftRepository.CreateRawPushpayRecurrentGiftSchedule(recurringGift);
            }
            _logger.Info($"PullRecurringGiftsAsync is complete.  Start Date: {startDate}, End Date: {endDate}");
        }

        // TODO: Make the argument be of PushPayTransactionBaseDTO if external links gets moved there.
        public int? ParseFundIdFromExternalLinks(PushpayRecurringGiftDto schedule)
        {
            if (!schedule.ExternalLinks.Any()) return null;
            var externalLink = schedule.ExternalLinks
                .FirstOrDefault(e => e.Relationship.ToLower() == "fund_id");
            return externalLink?.Value;

        }
    }
}
