using System;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class DepositDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("batchCount")]
        public int BatchCount { get; set; }

        [JsonProperty("depositDateTime")]
        public DateTime DepositDateTime { get; set; }

        [JsonProperty("depositName")]
        public string DepositName { get; set; }

        [JsonProperty("depositTotalAmount")]
        public decimal DepositTotalAmount { get; set; }

        [JsonProperty("depositAmount")]
        public decimal DepositAmount { get; set; }

        [JsonProperty("exported")]
        public bool Exported { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("processorTransferId")]
        public string ProcessorTransferId { get; set; }

        [JsonProperty("vendorDetailUrl")]
        public string VendorDetailUrl { get; set; }
    }
}
