using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class AmountDto
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }
    }
}
