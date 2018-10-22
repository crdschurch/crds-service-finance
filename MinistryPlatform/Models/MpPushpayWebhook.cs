using System;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "cr_Pushpay_Webhooks")]
    public class MpPushpayWebhook
    {
        [JsonProperty("Pushpay_Webhook_ID")]
        public int? PushpayWebhookId { get; set; }

        [JsonProperty("Date_Time")]
        public DateTime? DateTime { get; set; }

        [JsonProperty("Event_Type")]
        public string EventType { get; set; }

        [JsonProperty("Payload")]
        public string Payload { get; set; }
    }
}
