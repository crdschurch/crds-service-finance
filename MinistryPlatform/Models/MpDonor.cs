using System;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Donors")]
    public class MpDonor
    {
        [JsonProperty("Donor_ID")]
        public int DonorId { get; set; }

        [JsonProperty("Contact_ID")]
        public int ContactId { get; set; }

        [JsonProperty("Statement_Frequency_ID")]
        public int StatementFrequencyId { get; set; }

        [JsonProperty("Statement_Type_ID")]
        public int StatementTypeId { get; set; }

        [JsonProperty("Setup_Date")]
        public DateTime SetupDate { get; set; }
    }
}
