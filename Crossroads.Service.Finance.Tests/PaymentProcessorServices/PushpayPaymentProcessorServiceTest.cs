using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services;
using Crossroads.Web.Common.Configuration;
using Moq;
using RestSharp;
using Xunit;

namespace Crossroads.Service.Finance.Test.PaymentProcessorServices
{
    public class PushpayPaymentProcessorServiceTest
    {
        private readonly Mock<IRestClient> _restClient;
        private readonly PushpayPaymentProcessorService _fixture;

        public PushpayPaymentProcessorServiceTest()
        {
            _restClient = new Mock<IRestClient>(MockBehavior.Strict);

            _fixture = new PushpayPaymentProcessorService(_restClient.Object);
        }

        [Fact]
        public void ShouldGetChargesForTransfer()
        {
            // Arrange
            var settlementKey = "111aaa222bbb";

            var paymentsDto = new PaymentsDto
            {
                TotalPages = 45,
                payments = new List<PaymentProcessorChargeDto>()
            };

            var response = new Mock<IRestResponse<PaymentsDto>>();
            response.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            response.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            response.SetupGet(mocked => mocked.Data).Returns(paymentsDto);

            _restClient.Setup(mocked => mocked.Execute<PaymentsDto>(It.IsAny<IRestRequest>())).Returns(response.Object);
            _restClient.Setup(m => m.Execute(It.IsAny<RestRequest>())).Returns(response.Object);

            // Act
            var result = _fixture.GetChargesForTransfer(settlementKey);

            // Assert
            Assert.NotNull(result);
        }
    }
}
