using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayAnticipatedPaymentField
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonProperty("readOnly")]
        public bool ReadOnly { get; set; }
    }
}