using System;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Crossroads.Web.Common.Configuration;
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
            return _donationService.UpdateDonation(donation);
        }
    }
}
