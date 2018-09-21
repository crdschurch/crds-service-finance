using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services.Recurring
{
    public interface IRecurringService
    {
        List<string> SyncRecurringGifts(DateTime startDateTime, DateTime endDateTime);
    }
}
