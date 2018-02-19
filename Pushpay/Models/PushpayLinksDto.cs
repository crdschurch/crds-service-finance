using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayLinksDto
    {
        [JsonProperty("self")]
        public PushpayLinkDto Self { get; set; }

        [JsonProperty("merchant")]
        public PushpayLinkDto Merchant { get; set; }

        [JsonProperty("viewrecurringpayment")]
        public PushpayLinkDto ViewRecurringPayment { get; set; }

        [JsonProperty("status")]
        public PushpayLinkDto Status { get; set; }

        [JsonProperty("pay")]
        public PushpayLinkDto Pay { get; set; }

        [JsonProperty("payinapp")]
        public PushpayLinkDto PayInApp { get; set; }
    }
}
