using System;
using Microsoft.Azure.Cosmos.Table;

namespace Crossroads.Functions.Finance
{
    public class SyncLog : TableEntity
    {
        public string LogType { get { return this.PartitionKey; } }
        public string LogKey { get { return this.RowKey; } }
        public DateTime LogTimestamp { get; set; }
        public string LogStatus { get; set; }

        public SyncLog() { }
        public SyncLog(DateTime logTimestamp, string logStatus)
        {
            this.PartitionKey = "DonationSyncLog";
            this.RowKey = Guid.NewGuid().ToString();
            this.LogTimestamp = logTimestamp;
            this.LogStatus = logStatus;
        }
    }
}