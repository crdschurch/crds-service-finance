using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class PaymentDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("settlement")]
        public SettlementDto Settlement { get; set; }

        [JsonProperty("anticipatedPaymentToken")]
        public string AnticipatedPaymentToken { get; set; }

        [JsonProperty("recurringPaymentToken")]
        public string RecurringPaymentToken { get; set; }

        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("paymentToken")]
        public string PaymentToken { get; set; }

        [JsonProperty("amount")]
        public AmountDto Amount { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("_links")]
        public List<LinkDto> Links { get; set; }
    }
}