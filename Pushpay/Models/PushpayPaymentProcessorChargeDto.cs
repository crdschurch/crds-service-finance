using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayPaymentProcessorChargeDto
    {
        // TODO: Check the property name and type needed here
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("recurringPaymentToken")]
        public string RecurringPaymentToken { get; set; }

        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("paymentToken")]
        public string PaymentToken { get; set; }

        [JsonProperty("amount")]
        public PushpayAmountDto Amount { get; set; }
    }
}
