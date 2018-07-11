﻿using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayAmountDto
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }
    }
}
