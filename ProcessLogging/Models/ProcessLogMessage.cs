using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessLogging.Models
{
    public class ProcessLogMessage
    {
        //messageKey(defined in constants, not free-form),
        public string MessageType { get; set; }

        //messageVersion(semantically versioned),
        public string MessageVersion { get; set; }

        //messageTime,
        public DateTime MessageTime { get; set; }

        //messageData(format for these is defined with the data point, provided in constants, and "versioned")
        public string MessageData { get; set; }

        public ProcessLogMessage(ProcessLogConstants.MessageType messageType)
        {
            MessageTime = DateTime.Now;
            MessageVersion = ProcessLogConstants.GetMessageVersion(messageType);
            MessageType = messageType.ToString();
        }
    }
}
