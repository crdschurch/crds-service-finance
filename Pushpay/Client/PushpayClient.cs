using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Newtonsoft.Json;
using Pushpay.Models;

namespace Pushpay
{
    public class PushpayClient
    {
        private HttpClient _httpClient;
        private string clientId = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_ID");
        private string clientSecret = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_SECRET");
        private Uri authUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_AUTH_ENDPOINT"));

        public PushpayClient()
        {
            _httpClient = new HttpClient();
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
                } else {
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
