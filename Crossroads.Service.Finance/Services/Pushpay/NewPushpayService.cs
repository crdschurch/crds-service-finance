﻿using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services.Congregations;
using Crossroads.Service.Finance.Services.DonorAccounts;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Newtonsoft.Json;
using NLog;
using Pushpay.Client;
using Pushpay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crossroads.Web.Common.Configuration;

namespace Crossroads.Service.Finance.Services
{
    //TODO: Rename this service and it's interface once the original files have been decommissioned
    public class NewPushpayService : INewPushpayService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPushpayClient _pushpayClient;
        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IDonationRepository _donationRepository;
        private readonly IDonationService _donationService;
        private readonly ICongregationService _congregationService;
        private readonly IDonationDistributionRepository _donationDistributionRepository;
        private readonly IDonorAccountService _donorAccountService;

        private readonly int _mpDonationStatusPending, _mpDonationStatusDeposited, _mpDonationStatusDeclined, _mpDonationStatusSucceeded,
	        _mpPushpayRecurringWebhookMinutes, _mpDefaultContactDonorId, _mpNotSiteSpecificCongregationId;
        private const int MaxRetryMinutes = 15;
        private const int PushpayProcessorTypeId = 1;
        private const int NotSiteSpecificCongregationId = 5;
        private readonly string CongregationFieldKey = Environment.GetEnvironmentVariable("PUSHPAY_SITE_FIELD_KEY");

        private Dictionary<string, int> _recurringGiftStatuses = new Dictionary<string, int>
        {
	        { "Active", 1 },
	        { "Paused", 2 },
	        { "Cancelled", 3 }
        };

        public NewPushpayService(IPushpayClient pushpayClient, IRecurringGiftRepository recurringGiftRepository, IDonationRepository donationRepository,
	        IDonationService donationService, ICongregationService congregationService, IDonorAccountService donorAccountService, IDonationDistributionRepository donationDistributionRepository,
	        IConfigurationWrapper configurationWrapper)
        {
            _pushpayClient = pushpayClient;
            _recurringGiftRepository = recurringGiftRepository;
            _donationRepository = donationRepository;
            _donationService = donationService;
            _congregationService = congregationService;
            _donorAccountService = donorAccountService;
            _donationDistributionRepository = donationDistributionRepository;

            _mpDonationStatusPending = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusPending") ?? 1;
            _mpDonationStatusDeposited = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusDeposited") ?? 2;
            _mpDonationStatusDeclined = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusDeclined") ?? 3;
            _mpDonationStatusSucceeded = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusSucceeded") ?? 4;
            _mpDefaultContactDonorId = configurationWrapper.GetMpConfigIntValue("COMMON", "defaultDonorID") ?? 1;
            _mpPushpayRecurringWebhookMinutes = configurationWrapper.GetMpConfigIntValue("CRDS-FINANCE", "PushpayJobDelayMinutes") ?? 1;
            _mpNotSiteSpecificCongregationId = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "NotSiteSpecific") ?? 5;
        }

        public async Task PullRecurringGiftsAsync(DateTime startDate, DateTime endDate)
        {
            _logger.Info($"PullRecurringGiftsAsync is starting.  Start Date: {startDate}, End Date: {endDate}");
            var recurringGifts = await _pushpayClient.GetRecurringGiftsAsync(startDate, endDate);
            _logger.Info($"Got {recurringGifts.Count} updates to recurring gift schedules and/or new schedules from PushPay.");
            foreach (var recurringGift in recurringGifts)
            {
                _recurringGiftRepository.CreateRawPushpayRecurrentGiftSchedule(recurringGift);
            }
            _logger.Info($"PullRecurringGiftsAsync is complete.  Start Date: {startDate}, End Date: {endDate}");
        }

        // TODO: Make the argument be of PushPayTransactionBaseDTO if external links gets moved there.
        public int? ParseFundIdFromExternalLinks(PushpayRecurringGiftDto schedule)
        {
            if (!schedule.ExternalLinks.Any()) return null;
            var externalLink = schedule.ExternalLinks
                .FirstOrDefault(e => e.Relationship.ToLower() == "fund_id");
            return externalLink?.Value;
        }

	    public async Task PollDonationsAsync(string lastSuccessfulRunTime)
        {
	        var startTime = DateTime.Parse(lastSuccessfulRunTime).AddMinutes(-2);

            var donations = await _pushpayClient.GetPolledDonationsJson(startTime, DateTime.Now);

            foreach (var donation in donations)
            {
                _donationRepository.CreateRawPushpayDonation(donation);
            }
            _logger.Info($"PollDonationsAsync is complete.  Start Time: {startTime}, End Time: {DateTime.Now}");
        }

        public async Task ProcessRawDonations()
        {
	        int? lastSyncIndex = null;
	        do
	        {
		        var donationsToProcess = await _donationRepository.GetUnprocessedDonations(lastSyncIndex);
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
			        Task.WaitAll(setOfDonationsToProcess.Select(ProcessDonation).ToArray());
		        }
	        } while (lastSyncIndex.HasValue);
        }

        public async Task<MpDonation> ProcessDonation(MpRawDonation mpRawDonation)
        {
	        var pushpayPaymentDto = JsonConvert.DeserializeObject<PushpayPaymentDto>(mpRawDonation.RawJson);

            var mpDonation = await _donationRepository.GetDonationByTransactionCode($"PP-{pushpayPaymentDto.TransactionId}");

            // this may be a special case related to test code
            if (mpDonation == null)
            {
	            return null;
            }

            // add payment token to identify via api
            if (pushpayPaymentDto.PaymentToken != null)
            {
	            mpDonation.SubscriptionCode = pushpayPaymentDto.PaymentToken;
            }

            // set recurring gift id
            if (pushpayPaymentDto.RecurringPaymentToken != null)
            {
	            mpDonation.IsRecurringGift = true;

                var mpRecurringGift =
                    await _recurringGiftRepository.FindRecurringGiftBySubscriptionId(pushpayPaymentDto
                        .RecurringPaymentToken);

                if (mpRecurringGift == null)
                {
	                _logger.Error(
                        $"No recurring gift found by subscription id {pushpayPaymentDto.RecurringPaymentToken} when trying to attach it to donation");
                    Console.WriteLine(
                        $"No recurring gift found by subscription id {pushpayPaymentDto.RecurringPaymentToken} when trying to attach it to donation");
                }
                else
                {
	                mpDonation.RecurringGiftId = mpRecurringGift.RecurringGiftId;
                }
            }

            // if it doesn't exist, attach a donor account so we have access to payment details
            if (mpDonation.DonorAccountId == null)
            {
	            var mpDonorAccount = await _donorAccountService.GetOrCreateDonorAccount(pushpayPaymentDto, mpDonation.DonorId);
                mpDonation.DonorAccountId = mpDonorAccount.DonorAccountId;
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
                var refund = await _donationService.GetDonationByTransactionCode(pushpayPaymentDto.RefundFor.TransactionId);
                mpDonation.PaymentTypeId = refund.PaymentTypeId;
            }

            mpDonation.DonationStatusDate = DateTime.Now;

            // set the congregation on the donation distribution, based on the giver's site preference stated in pushpay
            // (this is a different business rule from soft credit donations) - default to using the id from the
            // webhook if possible so we don't have to mess with name matching
            int? congregationId = await _congregationService.LookupCongregationId(pushpayPaymentDto.PushpayFields, pushpayPaymentDto.Campus.Key);

            // set congregation
            if (congregationId != null)
            {
                var donationDistributions = await _donationDistributionRepository.GetByDonationId(mpDonation.DonationId);

                foreach (var donationDistribution in donationDistributions)
                {
                    donationDistribution.CongregationId = congregationId;
                    donationDistribution.HCDonorCongregationId = congregationId;
                }

                await _donationDistributionRepository.UpdateDonationDistributions(donationDistributions);
            }

            await _donationRepository.MarkAsProcessed(mpRawDonation);

            // save donation back to MP
            await _donationService.UpdateMpDonation(mpDonation);

            return mpDonation;
        }
    }
}
