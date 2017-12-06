using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class SettlementEventDto
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        //[JsonIgnore]
        [JsonProperty("totalAmount")]
        public AmountDto TotalAmount { get; set; }

        //[JsonIgnore]
        [JsonProperty("type")]
        public string Type { get; set; }

        //[JsonIgnore]
        [JsonProperty("totalPayments")]
        public int TotalPayments { get; set; }

        //[JsonIgnore]
        [JsonProperty("estimatedDepositDate")]
        public DateTime EstimatedDepositDate { get; set; }

        //[JsonIgnore]
        [JsonProperty("isReconciled")]
        public bool IsReconciled { get; set; }

        [JsonIgnore]
        //[JsonProperty("_links")]
        public List<LinkDto> Links { get; set; }
    }
}
