using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayAnticipatedFundDto
    {
        // backend should set this
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("taxDeductable")]
        public bool TaxDeductable { get; set; }
    }
}


