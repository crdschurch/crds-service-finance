using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class LinkDto
    {
        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
