using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Crossroads.Functions.Finance
{
    public static class DonationSync
    {
        [FunctionName("DonationSync")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"DonationSync timer trigger function executed at: {DateTime.Now}");
            var helper = new Helper("DonationSyncLog");

            var currentRunTime = DateTime.Now;
            var lastSuccessfulRunTime = await helper.GetLastSuccessfulRunTimeAsync();
            log.LogDebug($"Last Successful Runtime: {lastSuccessfulRunTime}");
            var httpStatusCode = await RunDonationEndpointAsync(lastSuccessfulRunTime.ToUniversalTime().ToString("u"), log);
            log.LogInformation($"HTTP Status Code: {httpStatusCode}");
            await helper.UpdateLogAsync(currentRunTime, httpStatusCode);

            log.LogInformation($"DonationSync returned a {httpStatusCode} response");

            var deletedRecordCount = await helper.ClearOldEntries(httpStatusCode, lastSuccessfulRunTime);
            log.LogInformation($"Deleted {deletedRecordCount} records out of DonationSyncLog.");
        }

        private static async Task<HttpStatusCode> RunDonationEndpointAsync(string lastSuccessfulRunTime, ILogger log)
        {
            string endpointUrl = Environment.GetEnvironmentVariable("ENDPOINT_URL") + "polling/donations";
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
            httpRequestMessage.Content = new StringContent("{\"lastSuccessfulRunTime\":\"" + lastSuccessfulRunTime + "\"}",
                Encoding.UTF8,
                "application/json");
            log.LogInformation($"Endpoint URL: {endpointUrl}");
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            return httpResponseMessage.StatusCode;
        }
    }
}
