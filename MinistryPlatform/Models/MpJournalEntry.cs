using System;
using System.Collections.Generic;
using System.Text;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "cr_Journal_Entries")]
    public class MpJournalEntry
    {
        [JsonProperty("Journal_Entry_ID")]
        public int JournalEntryID { get; set; }

        [JsonProperty("Created_Date")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("Exported_Date")]
        public DateTime ExportedDate { get; set; }

        [JsonProperty("GL_Account_Number")]
        public string GL_Account_Number { get; set; }

        [JsonProperty("Batch_ID")]
        public string BatchID { get; set; }

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Adjustment_Year")]
        public int AdjustmentYear { get; set; }

        [JsonProperty("Adjustment_Month")]
        public int AdjustmentMonth { get; set; }
    }
}
