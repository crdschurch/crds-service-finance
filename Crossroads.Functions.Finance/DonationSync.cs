using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Crossroads.Functions.Finance
{
    public static class DonationSync
    {
        [FunctionName("DonationSync")]
        public static async void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"DonationSync timer trigger function executed at: {DateTime.Now}");

            var currentRunTime = DateTime.Now;
            var lastSuccessfulRunTime = await GetLastSuccessfulRunTimeAsync();
            log.LogDebug($"Last Successful Runtime: {lastSuccessfulRunTime}");
            var httpStatusCode = await RunDonationEndpointAsync(lastSuccessfulRunTime.ToString(), log);
            log.LogInformation($"HTTP Status Code: {httpStatusCode}");
            UpdateLogAsync(currentRunTime, httpStatusCode);

            log.LogInformation($"DonationSync returned a {httpStatusCode} response");
        }

        private static async Task<DateTime> GetLastSuccessfulRunTimeAsync()
        {
            CloudTable donationSyncLogTable = await GetTableReference();

            var filterCondition = TableQuery.GenerateFilterCondition("LogStatus", QueryComparisons.Equal, "Success");
            var query = new TableQuery<DonationSyncLog>().Where(filterCondition);
            TableQuerySegment<DonationSyncLog> results = await donationSyncLogTable.ExecuteQuerySegmentedAsync(query, null); // used to be ExecuteQuery / ExecuteQueryAsync

            DateTime lastSuccessfulRuntime = DateTime.Now.AddMinutes(-60);
            if (results.Count() > 0)
            {
                results.OrderByDescending(l => l.LogTimestamp);
                lastSuccessfulRuntime = results.Results[0].LogTimestamp;
            }
            return lastSuccessfulRuntime;
        }

        private static async Task<HttpStatusCode> RunDonationEndpointAsync(string lastSuccessfulRunTime, ILogger log)
        {
            string endpointUrl = Environment.GetEnvironmentVariable("ENDPOINT_URL");
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
            httpRequestMessage.Content = new StringContent("{\"lastSuccessfulRunTime\":\"" + lastSuccessfulRunTime + "\"}",
                Encoding.UTF8,
                "application/json");
            log.LogInformation($"Endpoint URL: {endpointUrl}");
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            return httpResponseMessage.StatusCode;
        }

        private static async void UpdateLogAsync(DateTime currentRunTime, HttpStatusCode httpStatusCode)
        {
            CloudTable donationSyncLogTable = await GetTableReference();
            string logStatus = httpStatusCode == HttpStatusCode.NoContent ? "Success" : "Failed";
            var insertOperation = TableOperation.Insert(new DonationSyncLog(currentRunTime, logStatus));
            await donationSyncLogTable.ExecuteAsync(insertOperation);
        }

        private static async Task<CloudTable> GetTableReference()
        {
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("TABLE_STORAGE_CONNECTION"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var donationSyncLogTable = tableClient.GetTableReference("DonationSyncLog");
            await donationSyncLogTable.CreateIfNotExistsAsync();
            return donationSyncLogTable;
        }
    }
}