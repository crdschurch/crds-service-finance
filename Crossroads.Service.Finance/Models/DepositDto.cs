using System;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class DepositDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("account_number")]
        public string AccountNumber { get; set; }

        [JsonProperty("batch_count")]
        public int BatchCount { get; set; }

        [JsonProperty("deposit_date_time")]
        public DateTime DepositDateTime { get; set; }

        [JsonProperty("deposit_name")]
        public string DepositName { get; set; }

        [JsonProperty("deposit_total_amount")]
        public decimal DepositTotalAmount { get; set; }

        [JsonProperty("deposit_amount")]
        public decimal DepositAmount { get; set; }

        [JsonProperty("exported")]
        public bool Exported { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("processor_transfer_id")]
        public string ProcessorTransferId { get; set; }
    }
}
