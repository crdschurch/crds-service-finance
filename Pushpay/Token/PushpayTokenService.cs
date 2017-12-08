using System;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Pushpay.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace Pushpay.Token
{
    public class PushpayTokenService : IPushpayTokenService
    {
        private readonly string clientId = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_ID");
        private readonly string clientSecret = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_SECRET");
        private readonly Uri authUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_AUTH_ENDPOINT") ?? "https://auth.pushpay.com/pushpay-sandbox/oauth");
        private readonly Uri apiUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_API_ENDPOINT") ?? "https://sandbox-api.pushpay.io/v1");

        private readonly IRestClient _restClient;
        private const int RequestsPerSecond = 10;
        private const int RequestsPerMinute = 60;

        public PushpayTokenService(IRestClient restClient = null)
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

    }
}
