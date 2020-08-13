using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Crossroads.Functions.Finance
{
    public class RecurringScheduleSync
    {
        [FunctionName("RecurringScheduleSync")]
        public static async Task Run([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer, ILogger log)
        {
            var tableName = "RecurringScheduleSyncLog";
            log.LogInformation($"RecurringScheduleSync timer trigger function executed at: {DateTime.Now}");
            var helper = new Helper(tableName, HttpStatusCode.OK);

            var currentRunTime = DateTime.Now;
            var lastSuccessfulRunTime = await helper.GetLastSuccessfulRunTimeAsync();
            log.LogDebug($"Last Successful Runtime: {lastSuccessfulRunTime}");
            var httpStatusCode = await RunRecurringScheduleEndpointAsync(
                lastSuccessfulRunTime.ToUniversalTime().ToString("u"), currentRunTime.ToUniversalTime().ToString("u"),
                log);
            log.LogInformation($"HTTP Status Code: {httpStatusCode}");
            await helper.UpdateLogAsync(currentRunTime, httpStatusCode);

            log.LogInformation($"RecurringScheduleSync returned a {httpStatusCode} response");

            var deletedRecordCount = await helper.ClearOldEntries(httpStatusCode, lastSuccessfulRunTime);
            log.LogInformation($"Deleted {deletedRecordCount} records out of RecurringScheduleSyncLog.");
        }

        private static async Task<HttpStatusCode> RunRecurringScheduleEndpointAsync(string lastSuccessfulRunTime,
            string currentTime, ILogger log)
        {
            var endpointUrl = $"{Environment.GetEnvironmentVariable("ENDPOINT_URL")}Pushpay/updaterecurringgifts";
            var urlBuilder = new UriBuilder(endpointUrl);
            var query = HttpUtility.ParseQueryString(urlBuilder.Query);
            query["startDate"] = lastSuccessfulRunTime;
            query["endDate"] = currentTime;
            urlBuilder.Query = query.ToString();
            
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, urlBuilder.Uri);

            log.LogInformation($"Endpoint URL: {endpointUrl}");
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            return httpResponseMessage.StatusCode;
        }
    }
}