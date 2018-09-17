
using System;
using System.Collections.Generic;
namespace Utilities.Logging
{
    public class LogEventEntry
    {
        private LogEventType _logEventType;
        private Dictionary<string, object> _logEntryData;

        public LogEventType LogEventType => _logEventType;
        public Dictionary<string, object> LogEventData => _logEntryData;

        public LogEventEntry(LogEventType logEventType)
        {
            _logEventType = logEventType;
            _logEntryData = new Dictionary<string, object>();
        }

        public void Push(string key, Object objectData)
        {
            _logEntryData.Add(key, objectData);
        }
    }
}
