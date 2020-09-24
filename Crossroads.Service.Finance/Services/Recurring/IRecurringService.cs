using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinistryPlatform.Models;

namespace Crossroads.Service.Finance.Services.Recurring
{
    public interface IRecurringService
    {
        Task<List<string>> SyncRecurringGifts(DateTime startDateTime, DateTime endDateTime);
        Task SyncRecurringSchedules();
    }
}
