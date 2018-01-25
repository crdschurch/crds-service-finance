using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Crossroads.Service.Finance.Models
{
    public class PushpayWebhook
    {
        [JsonProperty("subscription")]
        public string Subscription { get; set; }

        [JsonProperty("events")]
        public IList<PushpayWebhookEvent> Events { get; set; }

        public DateTime IncomingTimeUtc { get; set; }
    }

    public class PushpayWebhookLinks
    {
        [JsonProperty("payment")]
        public string Payment { get; set; }

        [JsonProperty("merchant")]
        public string Merchant { get; set; }

        [JsonProperty("anticipantedpayment")]
        public string AnticipatedPayment { get; set; }

        [JsonProperty("recurringgift")]
        public string RecurringGift { get; set; }
    }

    public class PushpayWebhookEvent
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("eventType")]
        public string  EventType { get; set; }

        [JsonProperty("entityType")]
        public string EntityType { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("links")]
        public PushpayWebhookLinks Links { get; set; }
    }

}
