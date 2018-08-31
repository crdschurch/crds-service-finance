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
            Task sendToLogTask = new Task(SendToLog(logEventEntry));
        }

        async Task SendToLog(LogEventEntry logEventEntry)
        {
            await new Task(NewRelic.Api.Agent.NewRelic.RecordCustomEvent(logEventEntry.LogEventType.ToString(), logEventEntry.LogEventData));
        }
    }
}
