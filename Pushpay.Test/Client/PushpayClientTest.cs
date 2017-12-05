using Xunit;
using System.Net.Http;
using System.Reactive.Linq;
using Pushpay.Models;
using System.Net;
using System;
using Newtonsoft.Json;
using System.Text;

namespace Pushpay.Test
{
    public class PushpayClientTest
    {
        private readonly HttpClient _httpClient;
        private readonly PushpayClient _fixture;
        const string accessToken = "access-token-23894723";

        public PushpayClientTest()
        {
            var fakeResponseHandler = new FakeResponseHandler();
            var mockToken = new OAuth2TokenResponse()
            {
                AccessToken = accessToken
            };
            var mockResponseMessage = new HttpResponseMessage(){
                StatusCode = HttpStatusCode.OK,
            };
            mockResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(mockToken), Encoding.UTF8, "application/json");
            fakeResponseHandler.AddFakeResponse(new Uri("https://auth.pushpay.com/pushpay-sandbox/oauth/token"), mockResponseMessage);
            _httpClient = new HttpClient(fakeResponseHandler);
            _fixture = new PushpayClient(_httpClient);
        }

        [Fact]
        public void GetOAuthTokenTest()
        {
            var result = _fixture.GetOAuthToken().Wait();
            Assert.Equal(accessToken, result.AccessToken);
        }
    }
}
