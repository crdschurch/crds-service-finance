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
        private HttpClient _httpClient;
        private string clientId = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_ID");
        private string clientSecret = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_SECRET");
        private Uri authUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_AUTH_ENDPOINT") ?? "https://auth.pushpay.com/pushpay-sandbox/oauth");
        private Uri apiUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_API_ENDPOINT") ?? "https://sandbox-api.pushpay.io/v1");

        private readonly RestClient _restClient;
        private const int RequestsPerSecond = 10;
        private const int RequestsPerMinute = 60;

        // TODO: Consider switching wholly over to Rest Sharp
        public PushpayClient(HttpClient httpClient = null, RestClient restClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            _restClient = restClient ?? new RestClient();
        }

        public PushpayPaymentsDto GetPushpayDonations(string settlementKey)
        {
            Console.WriteLine("GetPushpayDonations");
            var tokenResponse = GetOAuthToken().Wait();
            _restClient.BaseUrl = apiUri;
            var request = new RestRequest(Method.POST);
            request.Resource = $"settlement/{settlementKey}/payments";
            Console.WriteLine(request.Resource);
            request.AddParameter("Authorization", string.Format("Bearer " + tokenResponse.AccessToken), ParameterType.HttpHeader);


            var paymentsDto = _restClient.Execute<PushpayPaymentsDto>(request).Data;

            // determine the delay needed to avoid hitting the rate limits for Pushpay
            var delay = 0;
            var totalPages = paymentsDto.TotalPages;

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

                // call and parse next load
                request.Resource = $"settlement/{settlementKey}/payments?page={i}";
                //request = new RestRequest(url, Method.GET);
                Console.WriteLine(request.Resource);
                var response = _restClient.Execute<PushpayPaymentsDto>(request);
                paymentsDto.payments.AddRange(response.Data.payments);
            }

            return paymentsDto;
        }

        public IObservable<OAuth2TokenResponse> GetOAuthToken()
        {
            return Observable.Create<OAuth2TokenResponse>(obs =>
            {
                _restClient.BaseUrl = authUri;
                _restClient.Authenticator = new HttpBasicAuthenticator(clientId, clientSecret);
                var request = new RestRequest(Method.POST);
                request.Resource = "token";
                request.AddParameter("grant_type", "client_credentials");
                request.AddParameter("scope", "read");
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
                    obs.OnError(new Exception("Authentication was not successful"));
                }
                return Disposable.Empty;
            });
        }

        // this is an example of how we can get token for a call to pushpay
        // TODO remove
        public IObservable<bool> DoStuff()
        {
            var token = GetOAuthToken().Wait();
            Console.WriteLine("token");
            Console.WriteLine(token.AccessToken);
            return Observable.Create<bool>(obs =>
            {
                obs.OnNext(true);
                obs.OnCompleted();
                return Disposable.Empty;
            });
        }

    }
}
