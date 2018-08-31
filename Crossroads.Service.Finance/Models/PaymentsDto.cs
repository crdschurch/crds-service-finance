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

        [JsonProperty("total")]
        public int Total { get; set; }

        // RestSharp currently has an issue deserializing List not named the same as property,
        // so have to name this "items" in order to deserialize properly
        //
        // ReSharper disable once InconsistentNaming
        [JsonProperty("items")]
        public List<PaymentDto> items { get; set; }
    }
}
