using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Interfaces;
using MinistryPlatform.Interfaces;
using Moq;
using Pushpay.Client;
using Xunit;

namespace Crossroads.Service.Finance.Test.NewPushpayService
{
	public class NewPushpayServiceTest
	{
		private readonly Mock<IPushpayClient> _pushpayClient;
		private readonly Mock<IRecurringGiftRepository> _recurringGiftRepository;
		private readonly Mock<IDonationRepository> _donationRepository;


		public INewPushpayService _fixture;

		public NewPushpayServiceTest()
		{
			_pushpayClient = new Mock<IPushpayClient>();
			_recurringGiftRepository = new Mock<IRecurringGiftRepository>();
			_donationRepository = new Mock<IDonationRepository>();

			_fixture = new Services.NewPushpayService(_pushpayClient.Object, _recurringGiftRepository.Object, _donationRepository.Object);
		}

		[Fact]
		public void ShouldPullRecurringGiftsAsync_HandleNoSchedules()
		{
			// Arrange
			var startTime = new DateTime(2020, 8, 13, 2, 18, 00);
			var endtime = startTime.AddMinutes(5);

			_pushpayClient.Setup(r => r.GetRecurringGiftsAsync(startTime, endtime)).Returns(Task.FromResult(new List<string>()));

			// Act
			_fixture.PullRecurringGiftsAsync(startTime, endtime);

			// Assert
			_pushpayClient.VerifyAll();
		}

		[Fact]
		public void ShouldPullRecurringGiftsAsync_HandleSchedules()
		{
			// Arrange
			var startTime = new DateTime(2020, 8, 13, 2, 18, 00);
			var endtime = startTime.AddMinutes(5);

			var schedules = new List<string>
			{
				"json"
			};

			_pushpayClient.Setup(r => r.GetRecurringGiftsAsync(startTime, endtime)).Returns(Task.FromResult(schedules));
			_recurringGiftRepository.Setup(r => r.CreateRawPushpayRecurrentGiftSchedule(It.IsAny<string>()));

			// Act
			_fixture.PullRecurringGiftsAsync(startTime, endtime);

			// Assert
			_pushpayClient.VerifyAll();
			_recurringGiftRepository.VerifyAll();
		}

		[Fact]
		public void ShouldPullDonationsAsync_HandleNoSchedules()
		{
			// Arrange
			var startTime = new DateTime(2020, 8, 13, 2, 18, 00);

			_pushpayClient.Setup(r => r.GetPolledDonationsJson(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(Task.FromResult(new List<string>()));

			// Act
			_fixture.PollDonationsAsync(startTime.ToString());

			// Assert
			_pushpayClient.VerifyAll();
		}

		[Fact]
		public void ShouldPullDonationsAsync_HandleSchedules()
		{
			// Arrange
			var startTime = new DateTime(2020, 8, 13, 2, 18, 00);

			var schedules = new List<string>
			{
				"json"
			};

			_pushpayClient.Setup(r => r.GetPolledDonationsJson(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(Task.FromResult(schedules));
			_donationRepository.Setup(r => r.CreateRawPushpayDonation(It.IsAny<string>()));

			// Act
			_fixture.PollDonationsAsync(startTime.ToString());

			// Assert
			_pushpayClient.VerifyAll();
		}
	}
}
