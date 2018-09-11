using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pushpay
{
    public class PushpayResponseBaseDto
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("items")]
        public List<dynamic> items { get; set; }
    }
}
