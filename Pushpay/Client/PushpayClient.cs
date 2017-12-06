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

namespace Pushpay
{
    public class PushpayClient : IPushpayClient
    {
        private HttpClient _httpClient;
        private string clientId = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_ID");
        private string clientSecret = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_SECRET");
        private Uri authUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_AUTH_ENDPOINT") ?? "https://auth.pushpay.com/pushpay-sandbox/oauth/token");

        private readonly IRestClient _restClient;
        private const int RequestsPerSecond = 10;
        private const int RequestsPerMinute = 60;

        // TODO: Consider switching wholly over to Rest Sharp
        public PushpayClient(HttpClient httpClient = null, IRestClient restClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            _restClient = restClient;
        }

        public PushpayPaymentsDto GetPushpayDonations(string settlementKey)
        {
            var token = GetOAuthToken().Wait();

            var url = $"settlement/{settlementKey}/payments";
            var request = new RestRequest(url, Method.GET);

            //request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("grant_type", "client_credentials");
            //request.AddParameter("client_id", "client-app");
            //request.AddParameter("client_secret", "secret");

            request.AddParameter("Authorization",
                string.Format("Bearer " + token.AccessToken),
                ParameterType.HttpHeader);

            //request.

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
                url = $"settlement/{settlementKey}/payments?page={i}";
                request = new RestRequest(url, Method.GET);
                var response = _restClient.Execute<PushpayPaymentsDto>(request);
                paymentsDto.payments.AddRange(response.Data.payments);
            }

            return paymentsDto;
        }

        public IObservable<OAuth2TokenResponse> GetOAuthToken()
        {
            return Observable.Create<OAuth2TokenResponse>(obs =>
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret)));
                var tokenRequestMessage = new HttpRequestMessage(HttpMethod.Post, authUri);

                var body = new Dictionary<string, string> {
                    {"grant_type", "client_credentials"},
                    {"scope", "list_my_merchants merchant:manage_community_members merchant:view_community_members merchant:view_payments merchant:view_recurring_payments organization:manage_funds read"}
                };

                tokenRequestMessage.Content = new FormUrlEncodedContent(body);
                var tokenresponse = _httpClient.SendAsync(tokenRequestMessage);
                tokenresponse.Wait();

                if (tokenresponse.Result.StatusCode == HttpStatusCode.OK)
                {
                    var tokenJson = tokenresponse.Result.Content.ReadAsStringAsync();
                    var tokens = JsonConvert.DeserializeObject<OAuth2TokenResponse>(tokenJson.Result);
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
