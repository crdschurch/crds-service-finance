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
        public int Status { get; set; }

        [JsonProperty("anticipatedPaymentToken")]
        public string AnticipatedPaymentToken { get; set; }

        [JsonProperty("recurringPaymentToken")]
        public string RecurringPaymentToken { get; set; }

        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("paymentToken")]
        public string PaymentToken { get; set; }

        [JsonProperty("amount")]
        public PushpayAmountDto Amount { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("_links")]
        public List<PushpayLinkDto> Links { get; set; }
    }
}
