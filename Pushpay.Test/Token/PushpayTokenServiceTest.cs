using Xunit;
using System.Reactive.Linq;
using System.Net;
using System;
using System.Threading.Tasks;
using Moq;
using RestSharp;
using Newtonsoft.Json;
using Pushpay.Models;
using Pushpay.Token;

namespace Pushpay.Test
{
    public class PushpayTokenServiceTest
    {
        private readonly Mock<IRestClient> _restClient;
        private readonly PushpayTokenService _fixture;
        const string accessToken = "ryJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJwdXNocGV5IiwiYXVkIjoicHVzaHBheS1zYW5kYm94IiwibmJmIjoxNTEyNjgwMzgzLCJleAHiOjE1MTI2ODM5ODMsImNsaWVudF9pZCI6ImNyb3Nzcm9hZHMtaW5nYWdlLWRldi1jbGllbnQiLCJzY29wZSI6WyJyZWFkIiwiY3JlYXRlX2FudGljaXBhdGVkX3BheW1lbnQiXSwibWVyY2hhbnRzIjoiNzkwMzg4NCA3OTAyNjQ1In0.ffD4AaY-4Zd-o2nOG2OIcgwq327jSQPnry4kCKFql88";
        const string tokenType = "Bearer";
        const int expiresIn = 3600;

        public PushpayTokenServiceTest()
        {
            _restClient = new Mock<IRestClient>();

            _fixture = new PushpayTokenService(_restClient.Object);
        }

        private void SetupOauth()
        {
            dynamic pushpayResponseData = new System.Dynamic.ExpandoObject();
            pushpayResponseData.access_token = accessToken;
            pushpayResponseData.token_type = tokenType;
            pushpayResponseData.expires_in = expiresIn;
            pushpayResponseData.refresh_token = null;

            var tokenResponse = new OAuth2TokenResponse
            {
                AccessToken = accessToken,
                TokenType = tokenType,
                ExpiresIn = expiresIn,
                RefreshToken = null
            };

            //var x = new RestResponse<OAuth2TokenResponse>
            //{
            //    {
            //    }
            //}


            IRestResponse<OAuth2TokenResponse> restResponse = new RestResponse<OAuth2TokenResponse>
            {
                //Content = tokenResponse,
                StatusCode = HttpStatusCode.OK
            };

            _restClient.Setup(x => x.ExecuteTaskAsync<OAuth2TokenResponse>(It.IsAny<IRestRequest>()))
                .Returns(Task.FromResult(restResponse));
        }

        [Fact]
        public void GetOAuthTokenTest()
        {
            SetupOauth();

            var result = _fixture.GetOAuthToken().Result;

            Assert.Equal(accessToken, result.AccessToken);
            Assert.Equal(tokenType, result.TokenType);
            Assert.Equal(expiresIn, result.ExpiresIn);
            Assert.Null(result.RefreshToken);
        }

        [Fact]
        public async void GetOAuthTokenTestFailure()
        {
            //_restClient.Setup(x => x.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<Action<IRestResponse>>()))
            //    .Returns(new RestResponse<object>
            //    {
            //        StatusCode = HttpStatusCode.BadRequest
            //    });

            _restClient.Setup(x => x.ExecuteAsync<object>(
                    Moq.It.IsAny<IRestRequest>(),
                    Moq.It.IsAny<Action<IRestResponse<object>, RestRequestAsyncHandle>>()))
                .Callback<IRestRequest, Action<IRestResponse<object>, RestRequestAsyncHandle>>((request, callback) =>
                {
                    var responseMock = new Mock<IRestResponse<object>>();
                    responseMock.Setup(r => r.Data).Returns(new RestResponse<object>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorException = new Exception()
                    });
                    callback(responseMock.Object, null);
                });

                //await Assert.ThrowsAsync<Exception>(() => _fixture.GetOAuthToken());

                try
                {
                    var result = await Record.ExceptionAsync(() => _fixture.GetOAuthToken());
                }
                catch (Exception e)
                {
                    Assert.NotNull(e);
                    throw;
                }

                Assert.False(true);
        }
    }
}
