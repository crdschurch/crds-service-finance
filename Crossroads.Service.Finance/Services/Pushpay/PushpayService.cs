using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Pushpay.Client;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Services
{
    public class PushpayService : IPushpayService
    {
        private readonly IPushpayClient _pushpayClient;
        private readonly IDonationService _donationService;
        private readonly IMapper _mapper;

        public PushpayService(IPushpayClient pushpayClient, IDonationService donationService, IMapper mapper)
        {
            _pushpayClient = pushpayClient;
            _donationService = donationService;
            _mapper = mapper;
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

        public PaymentDto UpdatePayment(PushpayWebhook webhook)
        {
            var pushpayPayment = _pushpayClient.GetPayment(webhook);
            var donation = _donationService.GetDonationByTransactionCode(pushpayPayment.TransactionId);
            // TODO update payment stuff

            var updatedDonation = _donationService.UpdateDonation(donation);
            return _mapper.Map<PaymentDto>(updatedDonation);
        }
    }
}
