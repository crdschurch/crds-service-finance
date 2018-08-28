using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class LinksDto
    {
        [JsonProperty("self")]
        public LinkDto Self { get; set; }

        [JsonProperty("merchant")]
        public LinkDto Merchant { get; set; }

        [JsonProperty("donorviewrecurringpayment")]
        public LinkDto ViewRecurringPayment { get; set; }

        [JsonProperty("merchantviewrecurringpayment")]
        public LinkDto MerchantViewRecurringPayment { get; set; }

        [JsonProperty("status")]
        public LinkDto Status { get; set; }

        [JsonProperty("pay")]
        public LinkDto Pay { get; set; }

        [JsonProperty("payinapp")]
        public LinkDto PayInApp { get; set; }
    }
}
