using Xunit;
using System.Net.Http;
using System.Reactive.Linq;
using System.Net;
using System;
using Moq;
using RestSharp;
using Pushpay.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pushpay.Test
{
    public class PushpayClientTest
    {
        private readonly PushpayClient _fixture;
        const string accessToken = "ryJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJwdXNocGV5IiwiYXVkIjoicHVzaHBheS1zYW5kYm94IiwibmJmIjoxNTEyNjgwMzgzLCJleAHiOjE1MTI2ODM5ODMsImNsaWVudF9pZCI6ImNyb3Nzcm9hZHMtaW5nYWdlLWRldi1jbGllbnQiLCJzY29wZSI6WyJyZWFkIiwiY3JlYXRlX2FudGljaXBhdGVkX3BheW1lbnQiXSwibWVyY2hhbnRzIjoiNzkwMzg4NCA3OTAyNjQ1In0.ffD4AaY-4Zd-o2nOG2OIcgwq327jSQPnry4kCKFql88";
        const string tokenType = "Bearer";
        const int expiresIn = 3600;
        private Mock<IRestClient> _restClient;

        public PushpayClientTest()
        {
            _restClient = new Mock<IRestClient>();

            _fixture = new PushpayClient(_restClient.Object);
        }

        [Fact]
        public void GetOAuthTokenTest()
        {
            dynamic pushpayResponseData = new System.Dynamic.ExpandoObject();
            pushpayResponseData.access_token = accessToken;
            pushpayResponseData.token_type = tokenType;
            pushpayResponseData.expires_in = expiresIn;
            pushpayResponseData.refresh_token = null;
            _restClient.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<object>
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonConvert.SerializeObject(pushpayResponseData)
                });

            var result = _fixture.GetOAuthToken().Wait();

            Assert.Equal(accessToken, result.AccessToken);
            Assert.Equal(tokenType, result.TokenType);
            Assert.Equal(expiresIn, result.ExpiresIn);
            Assert.Null(result.RefreshToken);
        }

        [Fact]
        public void GetOAuthTokenTestFailure()
        {
            _restClient.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<object>
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            Assert.Throws<Exception>(() => _fixture.GetOAuthToken().Wait());
        }
    }
}
