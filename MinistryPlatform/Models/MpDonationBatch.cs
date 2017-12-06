using Crossroads.Web.Common.MinistryPlatform;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Donations")]
    public class MpDonationBatch
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("batch_name")]
        public string BatchName { get; set; }

        [JsonProperty("setup_date_time")]
        public DateTime SetupDateTime { get; set; }

        [JsonProperty("batch_total_amount")]
        public decimal BatchTotalAmount { get; set; }

        [JsonProperty("item_count")]
        public int ItemCount { get; set; }

        [JsonProperty("batch_entry_type")]
        public int BatchEntryType { get; set; }

        [JsonProperty("deposit_id")]
        public int? DepositId { get; set; }

        [JsonProperty("finalized_date_time")]
        public DateTime FinalizedDateTime { get; set; }

        [JsonProperty("processor_transfer_id")]
        public string ProcessorTransferId { get; set; }

        [JsonProperty("program_id")]
        public int ProgramId { get; set; }

        [JsonProperty("batch_fee_total")]
        public decimal BatchFeeTotal { get; set; }
    }
}
