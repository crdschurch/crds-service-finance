using System;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
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

        private readonly IRestClient _restClient;

        public PushpayTokenService(IRestClient restClient = null)
        {
            _restClient = restClient ?? new RestClient();
            _restClient.BaseUrl = authUri;
        }

        public async Task<OAuth2TokenResponse> GetOAuthToken(string scope = "read")
        {
            ////return Observable.Create<OAuth2TokenResponse>(async obs =>
            ////{
            //    var taskCompletionSource = new TaskCompletionSource<OAuth2TokenResponse>();
            //    _restClient.Authenticator = new HttpBasicAuthenticator(clientId, clientSecret);
            //    var request = new RestRequest(Method.POST)
            //    {
            //        Resource = "token"
            //    };
            //    request.AddParameter("grant_type", "client_credentials");
            //    request.AddParameter("scope", scope);
            //    var result = _restClient.ExecuteAsync(request, response =>
            //    {
            //        if (response.StatusCode == HttpStatusCode.OK)
            //        {
            //            var tokenJson = response.Content;
            //            var tokens = JsonConvert.DeserializeObject<OAuth2TokenResponse>(tokenJson);
            //            taskCompletionSource.SetResult(tokens);
            //            //obs.OnNext(tokens);
            //            //obs.OnCompleted();
            //        }
            //        else
            //        {
            //            taskCompletionSource.SetException(new Exception());
            //            //obs.OnError(new Exception($"Authentication was not successful {response.StatusCode}"));
            //            //throw new Exception();
            //        }
            //    });

            //    return await taskCompletionSource.Task;
            ////});
            
            _restClient.Authenticator = new HttpBasicAuthenticator(clientId, clientSecret);
            var request = new RestRequest(Method.POST)
            {
                Resource = "token"
            };
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", scope);

            var response = await _restClient.ExecuteTaskAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var tokenJson = response.Content;
                var tokens = JsonConvert.DeserializeObject<OAuth2TokenResponse>(tokenJson);
                return tokens;
            }
            else
            {
                throw new Exception($"Authentication was not successful {response.StatusCode}");
            }
        }
    }
}
