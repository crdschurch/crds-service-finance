using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace Crossroads.Functions.Finance
{
    public class Helper
    {
        private readonly string tableName;

        public Helper(string tableName)
        {
            this.tableName = tableName;
        }
        
        public async Task<int> ClearOldEntries(HttpStatusCode httpStatusCode, DateTime lastSuccessful)
        {
            TableContinuationToken continuationToken = null;
            TableQuerySegment<SyncLog> results;
            var deleteTasks = new List<Task>();
            var deleteDate = DateTime.Now.AddDays(-5);
            deleteDate = httpStatusCode != HttpStatusCode.NoContent && deleteDate > lastSuccessful
                ? lastSuccessful.AddHours(-1)
                : deleteDate;

            var filter = TableQuery.GenerateFilterConditionForDate("LogTimestamp", QueryComparisons.LessThan, deleteDate);
            var donationSyncLogTable = await GetTableReference();
            var query = new TableQuery<SyncLog>().Where(filter);

            do
            {
                results = await donationSyncLogTable.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = results.ContinuationToken;
                deleteTasks.AddRange(results.Results.Select(TableOperation.Delete)
                    .Select(deleteOperation => donationSyncLogTable.ExecuteAsync(deleteOperation)));
            } while (results.ContinuationToken != null);

            Task.WaitAll(deleteTasks.ToArray());
            return deleteTasks.Count;
        }
        
        public async Task<DateTime> GetLastSuccessfulRunTimeAsync()
        {
            CloudTable donationSyncLogTable = await GetTableReference();

            var filterCondition = TableQuery.GenerateFilterCondition("LogStatus", QueryComparisons.Equal, "Success");
            var query = new TableQuery<SyncLog>().OrderByDesc("LogTimeStamp").Where(filterCondition);
            
            TableQuerySegment<SyncLog> results = await donationSyncLogTable.ExecuteQuerySegmentedAsync(query, null); // used to be ExecuteQuery / ExecuteQueryAsync

            DateTime lastSuccessfulRuntime = !results.Any() ? DateTime.Now.AddMinutes(-60): 
                results.OrderByDescending(l => l.LogTimestamp).ToList()[0].LogTimestamp;
            return lastSuccessfulRuntime;
        }
        
        public async Task<CloudTable> GetTableReference()
        {
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("TABLE_STORAGE_CONNECTION"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var donationSyncLogTable = tableClient.GetTableReference(tableName);
            await donationSyncLogTable.CreateIfNotExistsAsync();
            return donationSyncLogTable;
        }
        
        public async Task UpdateLogAsync(DateTime currentRunTime, HttpStatusCode httpStatusCode)
        {
            CloudTable donationSyncLogTable = await GetTableReference();
            string logStatus = httpStatusCode == HttpStatusCode.NoContent ? "Success" : "Failed";
            var insertOperation = TableOperation.Insert(new SyncLog(currentRunTime, logStatus));
            await donationSyncLogTable.ExecuteAsync(insertOperation);
        }

    }
}