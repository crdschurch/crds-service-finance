using System;
using System.Reactive.Linq;
using System.Threading;
using Pushpay.Models;
using Pushpay.Token;
using RestSharp;

namespace Pushpay.Client
{
    public class PushpayClient : IPushpayClient
    {
        private string clientId = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_ID");
        private string clientSecret = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_SECRET");
        private Uri authUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_AUTH_ENDPOINT") ?? "https://auth.pushpay.com/pushpay-sandbox/oauth");
        private Uri apiUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_API_ENDPOINT") ?? "https://sandbox-api.pushpay.io/v1");
        private string donationsScope = "read merchant:view_payments";

        private readonly IPushpayTokenService _pushpayTokenService;
        private readonly IRestClient _restClient;
        private const int RequestsPerSecond = 10;
        private const int RequestsPerMinute = 60;

        public PushpayClient(IPushpayTokenService pushpayTokenService, IRestClient restClient = null)
        {
            _pushpayTokenService = pushpayTokenService;
            _restClient = restClient ?? new RestClient();
        }

        public PushpayPaymentsDto GetPushpayDonations(string settlementKey)
        {
            var tokenResponse = _pushpayTokenService.GetOAuthToken(donationsScope).Wait();
            _restClient.BaseUrl = apiUri;
            var request = new RestRequest(Method.GET)
            {
                Resource = $"settlement/{settlementKey}/payments"
            };
            request.AddParameter("Authorization", string.Format("Bearer " + tokenResponse.AccessToken), ParameterType.HttpHeader);

            var response = _restClient.Execute<PushpayPaymentsDto>(request);
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.Data);
            Console.WriteLine(response.Content);

            var paymentsDto = response.Data;

            // determine if we need to call again (multiple pages), then
            // determine the delay needed to avoid hitting the rate limits for Pushpay
            if (paymentsDto == null) {
                throw new Exception($"Get Settlement from Pushpay not successful: {response.Content}");
            }

            var totalPages = paymentsDto.TotalPages;

            if (totalPages > 1)
            {
                var delay = 0;
                if (totalPages >= RequestsPerSecond && totalPages < RequestsPerMinute)
                {
                    delay = 150;
                }
                else if (totalPages >= RequestsPerMinute)
                {
                    delay = 1000;
                }

                for (int i = 0; i < totalPages; i++)
                {
                    Thread.Sleep(delay);
                    request.Resource = $"settlement/{settlementKey}/payments?page={i}";
                    response = _restClient.Execute<PushpayPaymentsDto>(request);
                    paymentsDto.Items.AddRange(response.Data.Items);
                }   
            }
            return paymentsDto;
        }

    }
}
