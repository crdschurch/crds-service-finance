using Crossroads.Service.Finance.Interfaces;
using MinistryPlatform.Interfaces;
using NLog;
using ProcessLogging.Transfer;
using Pushpay.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using ProcessLogging.Models;

namespace Crossroads.Service.Finance.Services
{
    //TODO: Rename this service and it's interface once the original files have been decommissioned
    public class NewPushpayService : INewPushpayService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPushpayClient _pushpayClient;
        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IDonationRepository _donationRepository;

        public NewPushpayService(IPushpayClient pushpayClient, IRecurringGiftRepository recurringGiftRepository, IDonationRepository donationRepository)
        {
            _pushpayClient = pushpayClient;
            _recurringGiftRepository = recurringGiftRepository;
            _donationRepository = donationRepository;
        }

        public async Task PullRecurringGiftsAsync(DateTime startDate, DateTime endDate)
        {
            _logger.Info($"PullRecurringGiftsAsync is starting.  Start Date: {startDate}, End Date: {endDate}");
            var recurringGifts = await _pushpayClient.GetRecurringGiftsAsync(startDate, endDate);
            foreach (var recurringGift in recurringGifts)
            {
                _recurringGiftRepository.CreateRawPushpayRecurrentGiftSchedule(recurringGift);
            }
            _logger.Info($"PullRecurringGiftsAsync is complete.  Start Date: {startDate}, End Date: {endDate}");
        }

        public async Task PollDonationsAsync(string lastSuccessfulRunTime)
        {
	        var startTime = DateTime.Parse(lastSuccessfulRunTime).AddMinutes(-2);

            var donations = await _pushpayClient.GetPolledDonationsJson(startTime, DateTime.Now);

            foreach (var donation in donations)
            {
                _donationRepository.CreateRawPushpayDonation(donation);
            }
            _logger.Info($"PollDonationsAsync is complete.  Start Time: {startTime}, End Time: {DateTime.Now}");
        }
    }
}
