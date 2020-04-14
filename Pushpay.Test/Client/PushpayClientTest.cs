using Moq;
using Pushpay.Client;
using Pushpay.Models;
using Pushpay.Token;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Pushpay.Test
{
    public class PushpayClientTest
    {
        private readonly Mock<IPushpayTokenService> _tokenService;
        private readonly Mock<IRestClient> _restClient;
        private readonly PushpayClient _fixture;
        const string accessToken = "ryJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJwdXNocGV5IiwiYXVkIjoicHVzaHBheS1zYW5kYm94IiwibmJmIjoxNTEyNjgwMzgzLCJleAHiOjE1MTI2ODM5ODMsImNsaWVudF9pZCI6ImNyb3Nzcm9hZHMtaW5nYWdlLWRldi1jbGllbnQiLCJzY29wZSI6WyJyZWFkIiwiY3JlYXRlX2FudGljaXBhdGVkX3BheW1lbnQiXSwibWVyY2hhbnRzIjoiNzkwMzg4NCA3OTAyNjQ1In0.ffD4AaY-4Zd-o2nOG2OIcgwq327jSQPnry4kCKFql88";
        const string tokenType = "Bearer";
        const int expiresIn = 3600;

        public PushpayClientTest()
        {
            _tokenService = new Mock<IPushpayTokenService>();
            _restClient = new Mock<IRestClient>();

            _fixture = new PushpayClient(_tokenService.Object, _restClient.Object);

            var token = new OAuth2TokenResponse()
            {
                AccessToken = "123"
            };
            _tokenService.Setup(r => r.GetOAuthToken(It.IsAny<string>())).Returns(Task.FromResult(token));
        }

        [Fact]
        public void GetPushpayDonationsTest()
        {
            var items = new List<object>();
            var item = new
            {
                Status = "pending"
            };
            items.Add(item);
            items.Add(item);

            _restClient.Setup(x => x.Execute<PushpayResponseBaseDto>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<PushpayResponseBaseDto>()
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = new PushpayResponseBaseDto()
                    {
                        Page = 1,
                        PageSize = 25,
                        TotalPages = 1,
                        items = items
                    }
                 });

            var result = _fixture.GetDonations("settlement-key-123");

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetPushpayDonationsSettlementDoesntExistTest()
        {
            _restClient.Setup(x => x.Execute<PushpayResponseBaseDto>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<PushpayResponseBaseDto>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorException = new System.Exception()
                });

            var result = Record.Exception(() => _fixture.GetDonations("settlement-key-123"));
            Assert.NotNull(result);
            Assert.IsType<AggregateException>(result);
        }

        [Fact]
        public void GetPaymentTest()
        {
            var webhook = Mock.PushpayStatusChangeRequestMock.Create();
            _restClient.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse()
                {
                    StatusCode = HttpStatusCode.OK
                });

            var result =  _fixture.GetPayment(webhook);
        }
         
        [Fact]
        public void ShouldGetDepositsByDateRange()
        {
            // Arrange
            var startDate = new DateTime(2017, 12, 6);
            var endDate = new DateTime(2017, 12, 13);

            var mockPushpayDepositDtos = new List<object>
            {
                new PushpaySettlementDto {},
                new PushpaySettlementDto {},
                new PushpaySettlementDto {},
                new PushpaySettlementDto {},
                new PushpaySettlementDto {},
                new PushpaySettlementDto {}
            };

            var pushpaySettlementResponseDto = new PushpayResponseBaseDto
            {
                Page = 0,
                PageSize = 5,
                TotalPages = 1,
                items = mockPushpayDepositDtos
            };

            _restClient.Setup(x => x.Execute<PushpayResponseBaseDto>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<PushpayResponseBaseDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = pushpaySettlementResponseDto
                });

            // Act
            var result = _fixture.GetDepositsByDateRange(startDate, endDate);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetRecurringGiftTest()
        {
            _restClient.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse()
                {
                    StatusCode = HttpStatusCode.OK
                });

            var result = _fixture.GetRecurringGift("https://resource.com");
        }
    }
}
