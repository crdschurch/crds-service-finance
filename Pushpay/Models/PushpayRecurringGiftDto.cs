using System;
using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayRecurringGiftDto: PushpayTransactionBaseDto
    {

        [JsonProperty("schedule")]
        public RecurringGiftSchedule Schedule { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }
        
        // TODO: See if this exists in all transaction. If so move to the base class
        [JsonProperty("externalLinks")]
        public ExternalLinks[] ExternalLinks { get; set; }
    }

    public class RecurringGiftSchedule
    {
        [JsonProperty("frequency")]
        public string Frequency{ get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
    }
}
