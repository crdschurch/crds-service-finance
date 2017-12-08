using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class PaymentsDto
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        // currently, Pushpay has this hard set to 25, but this could change
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("items")]
        public List<PaymentProcessorChargeDto> items { get; set; }
    }
}
