using System;
using System.Threading.Tasks;
using MinistryPlatform.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface INewPushpayService
    {
        Task PullRecurringGiftsAsync();
        Task PollDonationsAsync(DateTime? start_time = null, DateTime? end_time = null);
        int? ParseFundIdFromExternalLinks(PushpayRecurringGiftDto schedule);
        Task ProcessRawDonations();
        Task<int?> ProcessDonation(MpRawDonation mpRawDonation);
    }
}
