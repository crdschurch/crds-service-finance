using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayFieldValueDto
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
