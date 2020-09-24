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
        private readonly string _tableName;
        private readonly HttpStatusCode _successCode;

        public Helper(string tableName, HttpStatusCode successCode)
        {
            _tableName = tableName;
            _successCode = successCode;
        }
        
        public async Task<int> ClearOldEntries(HttpStatusCode httpStatusCode, DateTime lastSuccessful)
        {
            TableContinuationToken continuationToken = null;
            TableQuerySegment<SyncLog> results;
            var deleteTasks = new List<Task>();
            var deleteDate = DateTime.Now.AddDays(-5);
            deleteDate = httpStatusCode != _successCode && deleteDate > lastSuccessful
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
            var donationSyncLogTable = tableClient.GetTableReference(_tableName);
            await donationSyncLogTable.CreateIfNotExistsAsync();
            return donationSyncLogTable;
        }
        
        public async Task UpdateLogAsync(DateTime currentRunTime, HttpStatusCode httpStatusCode)
        {
            CloudTable donationSyncLogTable = await GetTableReference();
            string logStatus = httpStatusCode == _successCode ? "Success" : "Failed";
            var insertOperation = TableOperation.Insert(new SyncLog(currentRunTime, logStatus));
            await donationSyncLogTable.ExecuteAsync(insertOperation);
        }

        public static void SlackErrorNotification()
        {
            var urlWithAccessToken = Environment.GetEnvironmentVariable("SLACK_ERROR_URL");
            var environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT_NAME");
            var client = new SlackClient(urlWithAccessToken);

            client.PostMessage(
                username: "Azure Functions",
                text: $"{environmentName}: There was an error in the Azure DonationSync function.",
                channel: "#finance-ms-errors"
                );
        }
    }
}