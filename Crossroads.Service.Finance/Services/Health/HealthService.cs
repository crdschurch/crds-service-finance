using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services.Health
{
    public class HealthService : IHealthService
    {
        // This monitors hangfire connections and will force a restart of the service (if applicable)
        // if hangfire connections have been lost. This typically occurs when there has been a restart of 
        // the db server.

        // if a connection is lost/bad, this will try multiple times to connect, fail, and eventually
        // throw an exception
        public async Task<bool> GetHangfireStatus()
        {
            //var serializedDataTask = new Task<string>(() => SerializeJournalEntryStages(velosioJournalEntryBatch));

            var hangfireTask = Task.Run(() => Hangfire.JobStorage.Current.GetMonitoringApi().Servers());

            // this is not checking time on the heartbeat, but this is a possible addition in the future
            var hangfireServers = await hangfireTask;

            if (!hangfireServers.All(r => r.Heartbeat.HasValue))
            {
                return false;
            }

            return true;
        }
    }
}
