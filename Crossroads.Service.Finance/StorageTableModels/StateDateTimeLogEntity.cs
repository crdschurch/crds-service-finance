using System;
using Microsoft.Azure.Cosmos.Table;

namespace Crossroads.Service.Finance.StorageTableModels
{
    public class StateDateTimeLogEntity : TableEntity
    {
        public DateTime Value { get; set; }
        
        public StateDateTimeLogEntity()
        {
        }

        public StateDateTimeLogEntity(string appCode, string keyName)
        {
            PartitionKey = appCode;
            RowKey = keyName;
        }
    }
}