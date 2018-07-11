using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayFundDto
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("taxDeductible")]
        public bool TaxDeductible { get; set; }
    }
}
