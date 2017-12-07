using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Pushpay.Client;
using Pushpay.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace Pushpay
{
    public class PushpayClient : IPushpayClient
    {
        private string clientId = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_ID");
        private string clientSecret = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_SECRET");
        private Uri authUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_AUTH_ENDPOINT") ?? "https://auth.pushpay.com/pushpay-sandbox/oauth");
        private Uri apiUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_API_ENDPOINT") ?? "https://sandbox-api.pushpay.io/v1");
        private string donationsScope = "read merchant:view_payments";

        private readonly IRestClient _restClient;
        private const int RequestsPerSecond = 10;
        private const int RequestsPerMinute = 60;

        public PushpayClient(IRestClient restClient = null)
        {
            _restClient = restClient ?? new RestClient();
        }

        public IObservable<OAuth2TokenResponse> GetOAuthToken(string scope = "read")
        {
            return Observable.Create<OAuth2TokenResponse>(obs =>
            {
                _restClient.BaseUrl = authUri;
                _restClient.Authenticator = new HttpBasicAuthenticator(clientId, clientSecret);
                var request = new RestRequest(Method.POST)
                {
                    Resource = "token"
                };
                request.AddParameter("grant_type", "client_credentials");
                request.AddParameter("scope", scope);
                IRestResponse response = _restClient.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var tokenJson = response.Content;
                    var tokens = JsonConvert.DeserializeObject<OAuth2TokenResponse>(tokenJson);
                    obs.OnNext(tokens);
                    obs.OnCompleted();
                }
                else
                {
                    obs.OnError(new Exception($"Authentication was not successful {response.StatusCode}"));
                }
                return Disposable.Empty;
            });
        }

        public PushpayPaymentsDto GetPushpayDonations(string settlementKey)
        {
            var tokenResponse = GetOAuthToken(donationsScope).Wait();
            _restClient.BaseUrl = apiUri;
            var request = new RestRequest(Method.GET)
            {
                Resource = $"settlement/{settlementKey}/payments"
            };
            request.AddParameter("Authorization", string.Format("Bearer " + tokenResponse.AccessToken), ParameterType.HttpHeader);

            var response = _restClient.Execute<PushpayPaymentsDto>(request);
            var paymentsDto = response.Data;

            // determine if we need to call again (multiple pages), then
            // determine the delay needed to avoid hitting the rate limits for Pushpay
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
                    paymentsDto.items.AddRange(response.Data.items);
                }   
            }
            return paymentsDto;
        }

    }
}
