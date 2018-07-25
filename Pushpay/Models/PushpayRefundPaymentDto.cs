using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayRefundPaymentDto
    {
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("paymentToken")]
        public string PaymentToken { get; set; }
    }
}
