using Newtonsoft.Json;
using Pushpay.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Net;
using System.Threading.Tasks;
using Pushpay.Cache;

namespace Pushpay.Token
{
    public class PushpayTokenService : IPushpayTokenService
    {
        private readonly string clientId = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_ID");
        private readonly string clientSecret = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_SECRET");
        private readonly Uri authUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_AUTH_ENDPOINT") ?? "https://auth.pushpay.com/pushpay-sandbox/oauth");

        private readonly ICacheService _cacheService;
        private readonly IRestClient _restClient;

        public PushpayTokenService(ICacheService cacheService, IRestClient restClient = null)
        {
            _cacheService = cacheService;
            _restClient = restClient ?? new RestClient();
            _restClient.BaseUrl = authUri;
        }

        public async Task<OAuth2TokenResponse> GetOAuthToken(string scope = "read")
        {
            // check first to see if there is a cached and valid token
            var tokenString = _cacheService.Get(scope);

            if (tokenString == null)
            {
                // if token is not in cache, get a fresh one from Pushpay
                // TODO: determine how long their tokens are valid
                _restClient.Authenticator = new HttpBasicAuthenticator(clientId, clientSecret);
                var request = new RestRequest(Method.POST)
                {
                    Resource = "token"
                };
                request.AddParameter("grant_type", "client_credentials");
                request.AddParameter("scope", scope);

                var response = await _restClient.ExecuteTaskAsync<OAuth2TokenResponse>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Authentication was not successful {response.StatusCode}");
                }

                var tokenJson = response.Content;
                var tokens = JsonConvert.DeserializeObject<OAuth2TokenResponse>(tokenJson);
                tokenString = JsonConvert.SerializeObject(tokens);

                _cacheService.Set(scope, tokenString, new TimeSpan(0, 00, tokens.ExpiresIn - (int)(tokens.ExpiresIn * .25)));
                return tokens;
            }

            // serialize and return token here if it wasn't already pulled as part of the above process
            var returnTokens = JsonConvert.DeserializeObject<OAuth2TokenResponse>(tokenString);
            return returnTokens;
        }
    }
}
