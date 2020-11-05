using System;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services
{
    public interface ILastSyncService
    {
        Task<DateTime> GetLastDonationSyncTime();
        Task<DateTime> GetLastRecurringScheduleSyncTime();

        Task UpdateDonationSyncTime(DateTime newSyncTime);
        Task UpdateRecurringScheduleSyncTime(DateTime newSyncTime);
    }
}