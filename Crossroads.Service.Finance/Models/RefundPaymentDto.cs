using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class RefundPaymentDto
    {
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("paymentToken")]
        public string PaymentToken { get; set; }
    }
}