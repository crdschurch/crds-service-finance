using Crossroads.Web.Common.MinistryPlatform;
using System;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Batches")]
    public class MpDonationBatch
    {
        [JsonProperty("Batch_ID")]
        public int Id { get; set; }

        [JsonProperty("Batch_Name")]
        public string BatchName { get; set; }

        [JsonProperty("Setup_Date")]
        public DateTime SetupDateTime { get; set; }

        [JsonProperty("Batch_Total")]
        public decimal BatchTotalAmount { get; set; }

        [JsonProperty("Item_Count")]
        public int ItemCount { get; set; }

        [JsonProperty("Batch_Entry_Type_ID")]
        public int BatchEntryType { get; set; }

        [JsonProperty("Deposit_ID")]
        public int? DepositId { get; set; }

        [JsonProperty("Finalize_Date")]
        public DateTime FinalizedDateTime { get; set; }

        [JsonProperty("Processor_Transfer_ID")]
        public string ProcessorTransferId { get; set; }
    }
}
