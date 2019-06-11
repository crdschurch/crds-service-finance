using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayPaymentDto: PushpayTransactionBaseDto
    {
        [JsonProperty("settlement")]
        public PushpaySettlementDto Settlement { get; set; }

        [JsonProperty("recurringPaymentToken")]
        public string RecurringPaymentToken { get; set; }

        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        // this is on a payment when there is a refunded credit card
        //  payment and these details have the original payment
        [JsonProperty("refundFor")]
        public PushpayRefundPaymentDto RefundFor { get; set; }

        // this links to a payment that refunded this payment
        [JsonProperty("refundedBy")]
        public PushpayRefundPaymentDto RefundedBy { get; set; }

        [JsonProperty("paymentTypeId")]
        public int PaymentTypeId { get; set; }

        public bool IsRefund
        {
            get
            {
                return RefundFor != null && RefundFor.TransactionId != null;
            }
        }

        public bool IsStatusSuccess {
            get {
                return Status == PushpayPaymentStatus.Success;
            }
        }

        public bool IsStatusNew
        {
            get
            {
                return Status == PushpayPaymentStatus.New;
            }
        }

        public bool IsStatusProcessing
        {
            get
            {
                return Status == PushpayPaymentStatus.Processing;
            }
        }

        public bool IsStatusFailed
        {
            get
            {
                return Status == PushpayPaymentStatus.Failed;
            }
        }
    }
}
