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
    }

    public class RecurringGiftSchedule
    {
        [JsonProperty("frequency")]
        public string Frequency{ get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
    }
}
