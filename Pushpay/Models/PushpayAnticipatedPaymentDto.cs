using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pushpay.Models
{
    public class PushpayAnticipatedPaymentDto
    {
        // backend should set this
        [JsonProperty("merchantKey")]
        public string MerchantKey { get; set; }

        [JsonProperty("fields")]
        public List<PushpayAnticipatedPaymentField> Fields { get; set; }

        [JsonProperty("fund")]
        public JObject Fund { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("descriptionTitle")]
        public string DescriptionTitle { get; set; }

        [JsonProperty("returnUrl")]
        public string ReturnUrl { get; set; }

        [JsonProperty("returnTitle")]
        public string ReturnTitle { get; set; }

        [JsonProperty("_links")]
        public PushpayLinksDto Links { get; set; }
    }
}

