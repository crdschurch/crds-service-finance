using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public static class NewRelicAgentWrapper
    {
        public static void LogEvent(LogEventEntry logEventEntry)
        {
            NewRelic.Api.Agent.NewRelic.RecordCustomEvent(logEventEntry.EventType.ToString(), logEventEntry.LogEntryData);  
        }
    }

    public class LogEventEntry
    {
        public LogEventType EventType { get; set; }
        public Dictionary<string, object> LogEntryData { get; set; }
    }

    public enum LogEventType
    {
        StripeCancel
    }
}
