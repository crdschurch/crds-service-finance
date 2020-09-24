using System;
using System.Threading.Tasks;
using MinistryPlatform.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface INewPushpayService
    {
        Task PullRecurringGiftsAsync(DateTime startDate, DateTime endDate);
        Task PollDonationsAsync(string lastSuccessfulRunTime);
        int? ParseFundIdFromExternalLinks(PushpayRecurringGiftDto schedule);
        Task ProcessRawDonations();
        Task<MpDonation> ProcessDonation(MpRawDonation mpRawDonation);
    }
}
