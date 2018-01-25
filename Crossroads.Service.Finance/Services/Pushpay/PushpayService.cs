using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Crossroads.Web.Common.Configuration;
using Hangfire;
using log4net;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Pushpay.Client;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Services
{
    public class PushpayService : IPushpayService
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IPushpayClient _pushpayClient;
        private readonly IDonationService _donationService;
        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IMapper _mapper;
        private readonly int _mpDonationStatusPending, _mpDonationStatusDeclined, _mpDonationStatusSucceeded;
        private readonly int webhookDelayMinutes = 1;
        private readonly int maxRetryMinutes = 10;

        public PushpayService(IPushpayClient pushpayClient, IDonationService donationService, IMapper mapper, IConfigurationWrapper configurationWrapper, IRecurringGiftRepository recurringGiftRepository)
        {
            _pushpayClient = pushpayClient;
            _donationService = donationService;
            _mapper = mapper;
            _recurringGiftRepository = recurringGiftRepository;
            _mpDonationStatusPending = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusPending") ?? 1;
            _mpDonationStatusDeclined = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusDeclined") ?? 3;
            _mpDonationStatusSucceeded = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "DonationStatusSucceeded") ?? 4;
        }

        public PaymentsDto GetChargesForTransfer(string settlementKey)
        {
            var result = _pushpayClient.GetPushpayDonations(settlementKey);
            return _mapper.Map<PaymentsDto>(result);
        }

        private PaymentDto GetPayment(PushpayWebhook webhook)
        {
            var result = _pushpayClient.GetPayment(webhook);
            return _mapper.Map<PaymentDto>(result);
        }

        public void AddUpdateStatusJob(PushpayWebhook webhook)
        {
            // add incoming timestamp so that we can reprocess job for a
            //   certain amount of time
            webhook.IncomingTimeUtc = DateTime.UtcNow;
            AddUpdateDonationStatusFromPushpayJob(webhook);
        }

        private void AddUpdateDonationStatusFromPushpayJob(PushpayWebhook webhook)
        {
            BackgroundJob.Schedule(() => UpdateDonationStatusFromPushpay(webhook, true), TimeSpan.FromMinutes(webhookDelayMinutes));
        }

        public DonationDto UpdateDonationStatusFromPushpay(PushpayWebhook webhook, bool retry=false)
        {
            try {
                var pushpayPayment = _pushpayClient.GetPayment(webhook);
                // PushPay creates the donation a variable amount of time after the webhook comes in
                //   so it still may not be available
                var donation = _donationService.GetDonationByTransactionCode(pushpayPayment.TransactionId);
                if (pushpayPayment.IsStatusNew || pushpayPayment.IsStatusProcessing)
                {
                    donation.DonationStatusId = _mpDonationStatusPending;
                }
                else if (pushpayPayment.IsStatusSuccess)
                {
                    donation.DonationStatusId = _mpDonationStatusSucceeded;

                }
                else if (pushpayPayment.IsStatusFailed)
                {
                    donation.DonationStatusId = _mpDonationStatusDeclined;
                }
                _donationService.Update(donation);
                return donation;
            } catch (Exception e) {
                // donation not created by pushpay yet
                var now = DateTime.UtcNow;
                var webhookTime = webhook.IncomingTimeUtc;
                // if it's been less than ten minutes, try again in a minute
                if ((now - webhookTime).TotalMinutes < maxRetryMinutes && retry)
                {
                    AddUpdateDonationStatusFromPushpayJob(webhook);
                    // dont throw an exception as Hangfire tries to handle it
                    _logger.Error($"Payment: {webhook.Events[0].Links.Payment} not found in MP. Trying again in a minute.", e);
                    return null;
                }
                // it's been more than 10 minutes, let's chalk it up as PushPay
                //   ain't going to create it and call it a day
                else
                {
                    // dont throw an exception as Hangfire tries to handle it
                    _logger.Error($"Payment: {webhook.Events[0].Links.Payment} not found in MP after 10 minutes of trying. Giving up.", e);
                    return null;
                }
            }
        }

        public List<SettlementEventDto> GetDepositsByDateRange(DateTime startDate, DateTime endDate)
        {
            var result = _pushpayClient.GetDepositsByDateRange(startDate, endDate);
            return _mapper.Map<List<SettlementEventDto>>(result);
        }

        public PushpayAnticipatedPaymentDto CreateAnticipatedPayment(PushpayAnticipatedPaymentDto anticipatedPayment)
        {
            return _pushpayClient.CreateAnticipatedPayment(anticipatedPayment);
        }

        public void CreateRecurringGift(PushpayWebhook webhook)
        {
            var pushpayRecurringGift = _pushpayClient.GetRecurringGift(webhook.Events[0].Links.RecurringGift);
            var mpRecurringGift = _mapper.Map<MpRecurringGift>(pushpayRecurringGift);
            _recurringGiftRepository.CreateRecurringGift(mpRecurringGift);
        }
    }
}
