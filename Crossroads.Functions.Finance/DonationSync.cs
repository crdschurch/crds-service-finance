using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Crossroads.Functions.Finance
{
    public static class DonationSync
    {
        private static string _connectionString = Environment.GetEnvironmentVariable("TABLE_STORAGE_CONNECTION");   //"DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net;AccountName=laaz203tables;AccountKey=vsl0Zp7e9Do0Lhgg1etbiVcor0RabhfciBZKUblXJlvUw57Q6dW8pmV/z5vDmpPsMJ0iVPByYDXz8ljho8fGBw==";

        [FunctionName("DonationSync")]
        public static async void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var lastSuccessfulRunTime = GetLastSuccessfulRunTimeAsync();
            var currentRunTime = DateTime.Now;

            string endpointUrl = Environment.GetEnvironmentVariable("ENDPOINT_URL") + $"/{lastSuccessfulRunTime}";
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            HttpStatusCode httpStatusCode = httpResponseMessage.StatusCode;

            UpdateLogAsync(currentRunTime, httpStatusCode);

            log.LogInformation($"{endpointUrl} returned a {httpStatusCode} response");
        }

        private static async Task<DateTime> GetLastSuccessfulRunTimeAsync()
        {
            CloudTable donationSyncLogTable = await GetTableReference();

            var filterCondition = TableQuery.GenerateFilterCondition("LogStatus", QueryComparisons.Equal, "Success");
            var query = new TableQuery<DonationSyncLog>().Where(filterCondition);
            TableQuerySegment<DonationSyncLog> results = await donationSyncLogTable.ExecuteQuerySegmentedAsync(query, null); // used to be ExecuteQuery / ExecuteQueryAsync
            return results.Results[0].LogTimestamp;            
        }

        private static async void UpdateLogAsync(DateTime currentRunTime, HttpStatusCode httpStatusCode)
        {
            CloudTable donationSyncLogTable = await GetTableReference();
            string logStatus = httpStatusCode == HttpStatusCode.Created ? "Success" : "Failed";
            var insertOperation = TableOperation.Insert(new DonationSyncLog(currentRunTime, logStatus));
        }

        private static async Task<CloudTable> GetTableReference()
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var donationSyncLogTable = tableClient.GetTableReference("DonationSyncLog");
            await donationSyncLogTable.CreateIfNotExistsAsync();
            return donationSyncLogTable;
        }
    }
}
