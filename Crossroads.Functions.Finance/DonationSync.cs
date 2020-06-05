using System;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Crossroads.Functions.Finance
{
    public static class DonationSync
    {
        [FunctionName("DonationSync")]
        public static async void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string endpointUrl = Environment.GetEnvironmentVariable("ENDPOINT_URL");
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            HttpStatusCode httpStatusCode = httpResponseMessage.StatusCode;

            log.LogInformation($"{endpointUrl} returned a {httpStatusCode} response");
        }
    }
}
