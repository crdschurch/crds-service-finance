using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public class NewRelicAgentWrapper : IDataLoggingService
    {
        public void LogDataEvent(LogEventEntry logEventEntry)
        {
            Console.WriteLine(logEventEntry);
            //NewRelic.Api.Agent.NewRelic.RecordCustomEvent(logEventEntry.EventType.ToString(), logEventEntry.LogEntryData);  
        }
    }
}
