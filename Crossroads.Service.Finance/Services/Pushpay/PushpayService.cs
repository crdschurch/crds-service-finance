using System;
using System.Collections.Generic;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Crossroads.Web.Common.Configuration;
using Hangfire;
using Pushpay.Client;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Services
{
    public class PushpayService : IPushpayService
    {
        private readonly IPushpayClient _pushpayClient;
        private readonly IDonationService _donationService;
        private readonly IMapper _mapper;
        private readonly int _mpDonationStatusPending, _mpDonationStatusDeclined, _mpDonationStatusSucceeded;
        private readonly int webhookDelayMinutes = 2;

        public PushpayService(IPushpayClient pushpayClient, IDonationService donationService, IMapper mapper, IConfigurationWrapper configurationWrapper)
        {
            _pushpayClient = pushpayClient;
            _donationService = donationService;
            _mapper = mapper;
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

        public void AddUpdateDonationStatusFromPushpayJob(PushpayWebhook webhook)
        {
            BackgroundJob.Schedule(() => UpdateDonationStatusFromPushpay(webhook), TimeSpan.FromMinutes(webhookDelayMinutes));
        }

        public DonationDto UpdateDonationStatusFromPushpay(PushpayWebhook webhook)
        {
            var pushpayPayment = _pushpayClient.GetPayment(webhook);
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
    }
}
