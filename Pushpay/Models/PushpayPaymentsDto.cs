using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayPaymentsDto
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        // currently, Pushpay has this hard set to 25, but this could change
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("items")]
        public List<PushpayPaymentDto> Items { get; set; }

    }
}
