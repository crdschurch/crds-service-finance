using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpaySettlementResponseDto
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("items")]
        public List<PushpaySettlementDto> items { get; set; }
    }
}
