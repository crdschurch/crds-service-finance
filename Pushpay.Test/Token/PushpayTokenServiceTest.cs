using Xunit;
using System.Reactive.Linq;
using System.Net;
using System;
using System.Threading.Tasks;
using Moq;
using RestSharp;
using Newtonsoft.Json;
using Pushpay.Cache;
using Pushpay.Models;
using Pushpay.Token;

namespace Pushpay.Test
{
    public class PushpayTokenServiceTest
    {
        private readonly Mock<ICacheService> _cacheService;
        private readonly Mock<IRestClient> _restClient;
        private readonly PushpayTokenService _fixture;
        const string accessToken = "ryJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJwdXNocGV5IiwiYXVkIjoicHVzaHBheS1zYW5kYm94IiwibmJmIjoxNTEyNjgwMzgzLCJleAHiOjE1MTI2ODM5ODMsImNsaWVudF9pZCI6ImNyb3Nzcm9hZHMtaW5nYWdlLWRldi1jbGllbnQiLCJzY29wZSI6WyJyZWFkIiwiY3JlYXRlX2FudGljaXBhdGVkX3BheW1lbnQiXSwibWVyY2hhbnRzIjoiNzkwMzg4NCA3OTAyNjQ1In0.ffD4AaY-4Zd-o2nOG2OIcgwq327jSQPnry4kCKFql88";
        const string tokenType = "Bearer";
        const int expiresIn = 3600;

        public PushpayTokenServiceTest()
        {
            _cacheService = new Mock<ICacheService>();
            _restClient = new Mock<IRestClient>();

            _fixture = new PushpayTokenService(_cacheService.Object, _restClient.Object);
        }

        private void SetupOauth()
        {
            dynamic pushpayResponseData = new System.Dynamic.ExpandoObject();
            pushpayResponseData.access_token = accessToken;
            pushpayResponseData.token_type = tokenType;
            pushpayResponseData.expires_in = expiresIn;
            pushpayResponseData.refresh_token = null;
            var convertedResponse = JsonConvert.SerializeObject(pushpayResponseData);

            IRestResponse<OAuth2TokenResponse> restResponse = new RestResponse<OAuth2TokenResponse>
            {
                Content = convertedResponse,
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
            SetupOauth();

            IRestResponse<OAuth2TokenResponse> restResponse = new RestResponse<OAuth2TokenResponse>
            {
                StatusCode = HttpStatusCode.NotFound
            };

            _restClient.Setup(x => x.ExecuteTaskAsync<OAuth2TokenResponse>(It.IsAny<IRestRequest>()))
                .Returns(Task.FromResult(restResponse));

            var result = await Record.ExceptionAsync(() => _fixture.GetOAuthToken("test"));
            Assert.NotNull(result);
            Assert.IsType<Exception>(result);
        }

        [Fact]
        public async void GetTokenWithExpiredCache()
        {
            // arrange
            SetupOauth();

            IRestResponse<OAuth2TokenResponse> restResponse = new RestResponse<OAuth2TokenResponse>
            {
                StatusCode = HttpStatusCode.OK
            };

            var tokenString =
                "{\"access_token\":\"abc123def456\",\"token_type\":\"Bearer\",\"expires_in\":3600,\"refresh_token\":null}";

            _cacheService.Setup(r => r.Get(It.IsAny<string>())).Returns<string>(r => tokenString);

            _restClient.Setup(x => x.ExecuteTaskAsync<OAuth2TokenResponse>(It.IsAny<IRestRequest>()))
                .Returns(Task.FromResult(restResponse));


            // Act
            var result = _fixture.GetOAuthToken("test").Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OAuth2TokenResponse>(result);
        }

        [Fact]
        public async void GetTokenWithUnexpiredCache()
        {
            // arrange
            SetupOauth();

            var tokenString =
                "{\"access_token\":\"abc123def456\",\"token_type\":\"Bearer\",\"expires_in\":3600,\"refresh_token\":null}";

            _cacheService.Setup(r => r.Get(It.IsAny<string>())).Returns<string>(r => tokenString);

            // Act
            var result = _fixture.GetOAuthToken().Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OAuth2TokenResponse>(result);
        }
    }
}
