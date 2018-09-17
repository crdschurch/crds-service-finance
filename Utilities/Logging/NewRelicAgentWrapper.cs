using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Logging
{
    public class NewRelicAgentWrapper : IDataLoggingService
    {
        public void LogDataEvent(LogEventEntry logEventEntry)
        {
            Console.WriteLine(logEventEntry);
            NewRelic.Api.Agent.NewRelic.RecordCustomEvent(logEventEntry.LogEventType.ToString(),
                logEventEntry.LogEventData);
        }
    }
}
