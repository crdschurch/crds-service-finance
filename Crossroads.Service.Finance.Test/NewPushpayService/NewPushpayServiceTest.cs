﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services.Congregations;
using Crossroads.Service.Finance.Services.Donor;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Moq;
using Pushpay.Client;
using Pushpay.Models;
using Xunit;

namespace Crossroads.Service.Finance.Test.NewPushpayService
{
	public class NewPushpayServiceTest
	{
		private readonly Mock<IPushpayClient> _pushpayClient;
		private readonly Mock<IRecurringGiftRepository> _recurringGiftRepository;
		private readonly Mock<IDonationRepository> _donationRepository;
		private readonly Mock<IDonationService> _donationService;
		private readonly Mock<ICongregationService> _congregationService;
		private readonly Mock<IDonationDistributionRepository> _donationDistributionRepository;
		private readonly Mock<IDonorService> _donorService;
		private readonly Mock<IConfigurationWrapper> _configurationWrapper;

		public INewPushpayService _fixture;

		public NewPushpayServiceTest()
		{
			_pushpayClient = new Mock<IPushpayClient>();
			_recurringGiftRepository = new Mock<IRecurringGiftRepository>();
			_donationRepository = new Mock<IDonationRepository>();
			_donationService = new Mock<IDonationService>();
			_congregationService = new Mock<ICongregationService>();
			_donationDistributionRepository = new Mock<IDonationDistributionRepository>();
			_donorService = new Mock<IDonorService>();
			_configurationWrapper = new Mock<IConfigurationWrapper>();

			_fixture = new Services.NewPushpayService(_pushpayClient.Object, _recurringGiftRepository.Object, _donationRepository.Object,
				_donationService.Object, _congregationService.Object, _donorService.Object, _donationDistributionRepository.Object,
				_configurationWrapper.Object);
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

		[Fact]
		public void ShouldGetRawDonations()
		{
			// Arrange
			_donationRepository.Setup(r => r.GetUnprocessedDonations(It.IsAny<int?>()))
				.Returns(Task.FromResult(new List<MpRawDonation>()));

			// Act
			_fixture.ProcessRawDonations();

			// Assert
			_donationRepository.VerifyAll();
		}

		[Fact]
		public void ShouldProcessDonation()
		{
			// Arrange
			var rawDonation = Mock.PushpayRawPaymentMock.GetRawDonation();

			var mpRecurringGift = new MpRecurringGift
			{
				RecurringGiftId = 1234567
			};

			var mpDonationDistributions = new List<MpDonationDistribution>
			{
				new MpDonationDistribution
				{

				}
			};

			int? donorId = 1234567;

			_donationRepository.Setup(r => r.GetDonationByTransactionCode("PP-1111122222"))
				.Returns(Task.FromResult(new MpDonation()));
			_recurringGiftRepository.Setup(r => r.FindRecurringGiftBySubscriptionId("111111blahblahAAAAAA"))
				.Returns(Task.FromResult(mpRecurringGift));
			_donorService
				.Setup(r => r.FindDonorId(It.IsAny<PushpayTransactionBaseDto>()))
				.Returns(Task.FromResult(donorId));
			_donationService.Setup(r => r.FindDonorAccount(It.IsAny<PushpayTransactionBaseDto>(),
				It.IsAny<int>())).Returns(Task.FromResult(new MpDonorAccount()));
			_congregationService.Setup(r => r.LookupCongregationId(It.IsAny<List<PushpayFieldValueDto>>(), "Breeland"))
				.Returns(Task.FromResult(1));
			_donationDistributionRepository.Setup(r => r.GetByDonationId(It.IsAny<int>()))
				.Returns(Task.FromResult(new List<MpDonationDistribution>()));
			_donationDistributionRepository.Setup(r =>
				r.UpdateDonationDistributions(mpDonationDistributions));
			_donationRepository.Setup(r => r.MarkAsProcessed(It.IsAny<MpRawDonation>()));
			_donationService.Setup(r => r.UpdateMpDonation(It.IsAny<MpDonation>()));

			// Act
			var result = _fixture.ProcessDonation(rawDonation).Result;

			// Assert
			Assert.Equal(1234567, result.RecurringGiftId);
			_donationRepository.VerifyAll();
		}
	}
}