using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayLinkDto
    {
        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
