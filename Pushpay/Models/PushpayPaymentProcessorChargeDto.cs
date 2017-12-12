using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayPaymentDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("settlement")]
        public PushpaySettlementDto Settlement { get; set; }

        [JsonProperty("recurringPaymentToken")]
        public string RecurringPaymentToken { get; set; }

        [JsonProperty("fund")]
        public PushpayFundDto Fund{ get; set; }

        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("paymentToken")]
        public string PaymentToken { get; set; }

        [JsonProperty("amount")]
        public PushpayAmountDto Amount { get; set; }
    }
}
