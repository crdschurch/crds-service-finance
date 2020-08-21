using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Services.Congregations;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Congregations;
using MinistryPlatform.Models;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pushpay.Models;
using Xunit;

namespace Crossroads.Service.Finance.Test.Congregations
{
	public class CongregationServiceTest
	{
		private readonly Mock<ICongregationRepository> _congregationRepository;
		private readonly Mock<IConfigurationWrapper> _configurationWrapper;

		private readonly ICongregationService _fixture;

		public CongregationServiceTest()
		{
			_congregationRepository = new Mock<ICongregationRepository>();
			_configurationWrapper = new Mock<IConfigurationWrapper>();

			System.Environment.SetEnvironmentVariable("PUSHPAY_SITE_FIELD_KEY", "1234567654321");

			_fixture = new CongregationService(_congregationRepository.Object, _configurationWrapper.Object);
		}

		[Fact]
		public void ShouldLookupCongregationId()
		{
			// Arrange
			var pushpayPayment = JsonConvert.DeserializeObject<PushpayPaymentDto>(Mock.PushpayRawPaymentMock.GetPayment());

			var mpCongregations = new List<MpCongregation>
			{
				new MpCongregation
				{
					CongregationId = 1,
					CongregationName = "Breeland"
				}
			};

			_congregationRepository.Setup(r => r.GetCongregationByCongregationName(It.IsAny<string>()))
				.Returns(Task.FromResult(mpCongregations));

			// Act
			var result = _fixture.LookupCongregationId(pushpayPayment.PushpayFields, pushpayPayment.Campus.Key).Result;

			// Assert
			Assert.Equal(1, result);
		}
	}
}
