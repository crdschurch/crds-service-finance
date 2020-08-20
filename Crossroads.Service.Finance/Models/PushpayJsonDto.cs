using Newtonsoft.Json;
using System;

namespace Crossroads.Service.Finance.Models
{
    public class PushpayJsonDto
    {
        [JsonProperty(PropertyName = "pushpayJsonId")]
        public int PushpayJsonId { get; set; }
        
        [JsonProperty(PropertyName = "isProcessed")]
        public bool IsProcessed { get; set; }

        [JsonProperty(PropertyName = "isJsonValid")]
        public bool IsJsonValid { get; set; }

        [JsonProperty(PropertyName = "pulledFromPushpay")]
        public DateTime PulledFromPushpay { get; set; }
                
        [JsonProperty(PropertyName = "pushpayJson")]
        public string PushpayJson { get; set; }
    }
}
