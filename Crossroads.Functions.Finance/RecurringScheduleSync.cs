using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Crossroads.Functions.Finance
{
    public class RecurringScheduleSync
    {
        [FunctionName("RecurringScheduleSync")]
        public static async Task Run([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer, ILogger log)
        {
            var tableName = "RecurringScheduleSyncLog";
            log.LogInformation($"RecurringScheduleSync timer trigger function executed at: {DateTime.Now}");
            var helper = new Helper(tableName);

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
            string endpointUrl = Environment.GetEnvironmentVariable("ENDPOINT_URL") +
                                 $"Pushpay/updaterecurringgifts?startDate={lastSuccessfulRunTime}&endDate={currentTime}";
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, endpointUrl);

            httpRequestMessage.Content = new StringContent(
                "{\"lastSuccessfulRunTime\":\"" + lastSuccessfulRunTime + "\"}",
                Encoding.UTF8,
                "application/json");
            log.LogInformation($"Endpoint URL: {endpointUrl}");
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            return httpResponseMessage.StatusCode;
        }
    }
}