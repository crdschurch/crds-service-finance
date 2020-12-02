using System;
using System.Threading.Tasks;
using Crossroads.Service.Finance.StorageTableModels;
using Microsoft.Azure.Cosmos.Table;

namespace Crossroads.Service.Finance.Services
{
    public class LastSyncService : ILastSyncService
    {
        private const string TableName = "FinanceJobState";
        private const string PartitionKey = "CRDS-FINANCE";

        public async Task<DateTime> GetLastDonationSyncTime()
        {
            return await GetLastSyncTime("LastDonationSync");
        }

        public async Task<DateTime> GetLastRecurringScheduleSyncTime()
        {
            return await GetLastSyncTime("LastRecurringSync");
        }

        public async Task UpdateDonationSyncTime(DateTime newSyncTime)
        {
            var donationSyncEntry = new StateDateTimeLogEntity(PartitionKey, "LastDonationSync") {Value = newSyncTime};
            await CreateOrUpdateEntry(donationSyncEntry);
        }

        public async Task UpdateRecurringScheduleSyncTime(DateTime newSyncTime)
        {
            var recurringSynEntry = new StateDateTimeLogEntity(PartitionKey, "LastRecurringSync") {Value = newSyncTime};
            await CreateOrUpdateEntry(recurringSynEntry);
        }

        private async Task CreateOrUpdateEntry(StateDateTimeLogEntity entity)
        {
            var table = await GetTableReference();
            var insertOrMergeOption = TableOperation.InsertOrMerge(entity);
            await table.ExecuteAsync(insertOrMergeOption);
        }

        private async Task<DateTime> GetLastSyncTime(string keyName)
        {
            var table = await GetTableReference();
            var retrieveOptions = TableOperation.Retrieve<StateDateTimeLogEntity>(PartitionKey, keyName);
            var results = await table.ExecuteAsync(retrieveOptions);
            return results.Result is StateDateTimeLogEntity entity ? entity.Value : DateTime.Now.AddHours(-1);
        }

        private async Task<CloudTable> GetTableReference()
        {
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("TABLE_STORAGE_CONNECTION"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var donationSyncLogTable = tableClient.GetTableReference(TableName);
            await donationSyncLogTable.CreateIfNotExistsAsync();
            return donationSyncLogTable;
        }
    }
}