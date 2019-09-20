using System;
using System.Collections.Generic;
using System.Text;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "cr_Distribution_Adjustments")]
    public class MpDistributionAdjustment
    {
        [JsonProperty("Distribution_Adjustment_ID")]
        public int DistributionAdjustmentId { get; set; }

        [JsonProperty("Journal_Entry_ID")]
        public int? JournalEntryId { get; set; }

        [JsonProperty("Created_Date")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("Donation_Date")]
        public DateTime DonationDate { get; set; }

        [JsonProperty("Processed_Date")]
        public DateTime? ProcessedDate { get; set; }

        [JsonProperty("GL_Account_Number")]
        public string GLAccountNumber { get; set; }

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [JsonProperty("Adjustment")]
        public string Adjustment { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Donation_Distribution_ID")]
        public int? DonationDistributionID { get; set; }
    }
}
