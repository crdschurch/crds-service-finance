using System;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface INewPushpayService
    {
        Task PullRecurringGiftsAsync(DateTime startDate, DateTime endDate);
    }
}
