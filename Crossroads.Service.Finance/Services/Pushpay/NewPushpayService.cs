﻿using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services.Congregations;
using Crossroads.Service.Finance.Services.Donor;
using Crossroads.Service.Finance.Services.Slack;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Newtonsoft.Json;
using NLog;
using Pushpay.Client;
using Pushpay.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services
{
    //TODO: Rename this service and it's interface once the original files have been decommissioned
    public class NewPushpayService : INewPushpayService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ILastSyncService _lastSyncService;
        private readonly IPushpayClient _pushpayClient;
        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IDonationRepository _donationRepository;
        private readonly IDonationService _donationService;
        private readonly ICongregationService _congregationService;
        private readonly IDonationDistributionRepository _donationDistributionRepository;
        private readonly IDonorService _donorService;
        private readonly ISlackService _slackService;

        private readonly int _mpDonationStatusPending, _mpDonationStatusDeclined, _mpDonationStatusSucceeded;
        private readonly string _slackEndpoint;
        private readonly string _slackChannel;

        public NewPushpayService(IPushpayClient pushpayClient, IRecurringGiftRepository recurringGiftRepository, IDonationRepository donationRepository,
	        IDonationService donationService, ICongregationService congregationService, IDonorService donorService, IDonationDistributionRepository donationDistributionRepository,
	        IConfigurationWrapper configurationWrapper, ILastSyncService lastSyncService, ISlackService slackService)
        {
            _pushpayClient = pushpayClient;
            _recurringGiftRepository = recurringGiftRepository;
            _donationRepository = donationRepository;
            _donationService = donationService;
            _congregationService = congregationService;
            _donorService = donorService;
            _donationDistributionRepository = donationDistributionRepository;
            _lastSyncService = lastSyncService;
            _slackService = slackService;

			_mpDonationStatusPending = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusPending") ?? 1;
            _mpDonationStatusDeclined = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusDeclined") ?? 3;
            _mpDonationStatusSucceeded = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusSucceeded") ?? 4;

            _slackEndpoint = Environment.GetEnvironmentVariable("SLACK_ENDPOINT");
			_slackChannel = Environment.GetEnvironmentVariable("SLACK_CHANNEL");
		}

        public async Task PullRecurringGiftsAsync()
        {
	        var startDate = await _lastSyncService.GetLastRecurringScheduleSyncTime();
	        var endDate = DateTime.Now;
            _logger.Info($"PullRecurringGiftsAsync is starting.  Start Date: {startDate.ToLocalTime()}, End Date: {endDate}");
            var recurringGifts = await _pushpayClient.GetRecurringGiftsAsync(startDate, endDate);
            _logger.Info($"Got {recurringGifts.Count} updates to recurring gift schedules and/or new schedules from PushPay.");
            foreach (var recurringGift in recurringGifts)
            {
                _recurringGiftRepository.CreateRawPushpayRecurrentGiftSchedule(recurringGift);
            }
            _logger.Info($"PullRecurringGiftsAsync is complete.  Start Date: {startDate.ToLocalTime()}, End Date: {endDate}");
            await _lastSyncService.UpdateRecurringScheduleSyncTime(endDate);
        }

        // TODO: Make the argument be of PushPayTransactionBaseDTO if external links gets moved there.
        public int? ParseFundIdFromExternalLinks(PushpayRecurringGiftDto schedule)
        {
            if (!schedule.ExternalLinks.Any()) return null;
            var externalLink = schedule.ExternalLinks
                .FirstOrDefault(e => e.Relationship.ToLower() == "fund_id");
            return externalLink?.Value;
        }

	    public async Task PollDonationsAsync(DateTime? start_time = null, DateTime? end_time = null)
	    {
		    var startTime = (await _lastSyncService.GetLastDonationSyncTime()).AddMinutes(-2);
		    var endTime = DateTime.Now;

	        if (start_time != null && end_time != null)
	        {
		        startTime = start_time.GetValueOrDefault();
				endTime = end_time.GetValueOrDefault();
	        }

            var donations = await _pushpayClient.GetPolledDonationsJson(startTime, endTime);

            foreach (var donation in donations)
            {
                _donationRepository.CreateRawPushpayDonation(donation);
            }
            _logger.Info($"PollDonationsAsync is complete.  Start Time: {startTime.ToLocalTime()}, End Time: {endTime}");
            await _lastSyncService.UpdateDonationSyncTime(endTime);
	    }

        public async Task ProcessRawDonations()
        {
	        int? lastSyncIndex = null;
	        var totalCount = 0;
	        do
	        {
		        var donationsToProcess = await _donationRepository.GetUnprocessedDonationsFromProc(lastSyncIndex);
		        totalCount += donationsToProcess.Count;
		        _logger.Info($"Processing {donationsToProcess.Count} donations.");

		        // MP gets the top 1000 results. So we should get the last ID so we can get the next chunk of donations
		        lastSyncIndex = donationsToProcess.Count >= 1000
			        ? donationsToProcess.Last().DonationId
			        : (int?)null;

		        while (donationsToProcess.Any())
		        {
			        // Do a chunk at a time to not overload MP
			        Thread.Sleep(500);
			        var range = Math.Min(donationsToProcess.Count, 100);
			        var setOfDonationsToProcess = donationsToProcess.Take(range).ToList();
			        donationsToProcess.RemoveRange(0, range);

                    // Sync the chunk and wait for all to finish before processing the next chunk
                    var results = await Task.WhenAll(setOfDonationsToProcess.Select(ProcessDonation).ToArray());
                    var recordsToBeMarkedProcessed = results.Where(r => r.HasValue).Select(j => j.Value).ToList();
                    if (recordsToBeMarkedProcessed.Any())
                    {
						await _donationRepository.BatchMarkAsProcessed(recordsToBeMarkedProcessed);
                    }
		        }
	        } while (lastSyncIndex.HasValue);
	        _logger.Info($"Processed {totalCount} donations.");
        }

        public async Task<int?> ProcessDonation(MpRawDonation mpRawDonation)
        {
	        try
	        {
		        var pushpayPaymentDto = JsonConvert.DeserializeObject<PushpayPaymentDto>(mpRawDonation.RawJson);

		        var mpDonation =
			        await _donationRepository.GetDonationByTransactionCode($"PP-{pushpayPaymentDto.TransactionId}");

		        // this is to avoid having "bad" donations show up in MP
		        if (mpDonation == null && pushpayPaymentDto.IsStatusFailed)
		        {
			        return mpRawDonation.DonationId;
		        }

				// alert if the donation is not failed and doesn't exist in MP
				if (mpDonation == null && !pushpayPaymentDto.IsStatusFailed)
				{
					_slackService.SendSlackAlert(_slackEndpoint, _slackChannel, "Donation sync error", 
						$"Donation not in MP for transaction id: {pushpayPaymentDto.TransactionId}");

					// return a default so that it doesn't cause a null exception
					return null;
				}

				// add payment token to identify via api
		        if (pushpayPaymentDto.PaymentToken != null)
		        {
			        mpDonation.SubscriptionCode = pushpayPaymentDto.PaymentToken;
		        }

		        // set recurring gift id
				mpDonation.IsRecurringGift = pushpayPaymentDto.RecurringPaymentToken != null;
		        if (pushpayPaymentDto.RecurringPaymentToken != null)
		        {

			        var mpRecurringGift =
				        await _recurringGiftRepository.LookForRecurringGiftBySubscriptionId(pushpayPaymentDto
					        .RecurringPaymentToken);

			        if (mpRecurringGift == null)
			        {
				        _logger.Error($"No recurring gift found by subscription id {pushpayPaymentDto.RecurringPaymentToken} when trying to attach it to donation");
				        return null;
			        }
			        else
			        {
				        mpDonation.RecurringGiftId = mpRecurringGift.RecurringGiftId;
			        }
		        }

		        // attach donor account
		        var donorId = await _donorService.FindDonorId(pushpayPaymentDto);

		        if (donorId.HasValue)
		        {
			        mpDonation.DonorId = donorId.Value;
			        var donorAccount = await _donationService.FindDonorAccount(pushpayPaymentDto, donorId.Value);
			        mpDonation.DonorAccountId =
				        donorAccount?.DonorAccountId ??
				        (await _donationService.CreateDonorAccountFromPushpay(pushpayPaymentDto, donorId.Value))
				        .DonorAccountId;
		        }
		        else
		        {
			        var donor = await _donorService.CreateDonor(pushpayPaymentDto);
			        mpDonation.DonorId = donor.DonorId.Value;
			        mpDonation.DonorAccountId =
				        (await _donationService.CreateDonorAccountFromPushpay(pushpayPaymentDto, donor.DonorId.Value))
				        .DonorAccountId;
		        }

		        if (pushpayPaymentDto.IsStatusNew || pushpayPaymentDto.IsStatusProcessing)
		        {
			        mpDonation.DonationStatusId = _mpDonationStatusPending;
		        }
		        // only flip if not deposited
		        else if (pushpayPaymentDto.IsStatusSuccess && mpDonation.BatchId == null)
		        {
			        mpDonation.DonationStatusId = _mpDonationStatusSucceeded;
		        }
		        else if (pushpayPaymentDto.IsStatusFailed)
		        {
			        mpDonation.DonationStatusId = _mpDonationStatusDeclined;
		        }

		        // check if refund
		        if (pushpayPaymentDto.RefundFor != null)
		        {
			        // Set payment type for refunds
			        var refund =
				        await _donationService.GetDonationByTransactionCode($"PP-{pushpayPaymentDto.RefundFor.TransactionId}");
			        mpDonation.PaymentTypeId = refund.PaymentTypeId;
		        }

		        mpDonation.DonationStatusDate = DateTime.Now;

		        // set the congregation on the donation distribution, based on the giver's site preference stated in pushpay
		        // (this is a different business rule from soft credit donations) - default to using the id from the
		        // webhook if possible so we don't have to mess with name matching
		        int? congregationId = await _congregationService.LookupCongregationId(pushpayPaymentDto.PushpayFields,
			        pushpayPaymentDto.Campus.Key);

		        // set congregation
		        if (congregationId != null)
		        {
			        var donationDistributions =
				        await _donationDistributionRepository.GetByDonationId(mpDonation.DonationId);

			        foreach (var donationDistribution in donationDistributions)
			        {
				        donationDistribution.CongregationId = congregationId;
				        donationDistribution.HCDonorCongregationId = congregationId;
			        }

			        await _donationDistributionRepository.UpdateDonationDistributions(donationDistributions);
		        }

		        // save donation back to MP
		        await _donationService.UpdateMpDonation(mpDonation);

		        return mpRawDonation.DonationId;
	        }
	        catch (Exception ex)
	        {
		        _logger.Error($"Could not process donation {mpRawDonation}: {ex}");
	        }

	        return null;
        }
    }
}
