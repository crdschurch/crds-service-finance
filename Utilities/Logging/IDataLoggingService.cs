using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public interface IDataLoggingService
    {
        void LogDataEvent(LogEventEntry logEventEntry);
    }
}
