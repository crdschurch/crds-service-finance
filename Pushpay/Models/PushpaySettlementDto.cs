using System;
using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpaySettlementDto
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("estimatedDepositDate")]
        public DateTime EstimatedDepositDate { get; set; }

    }
}
