using System;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Donors")]
    public class MpDonor
    {
        [JsonProperty("Donor_ID")]
        public int? DonorId { get; set; }

        // this is what api_Common_FindMatchingContact proc
        //   calls the Donor_ID
        [JsonProperty("Donor_Record")]
        public int? DonorRecord { get; set; }

        [JsonProperty("Contact_ID")]
        public int ContactId { get; set; }

        [JsonProperty("Donor_Account_ID")]
        public int? DonorAccountId { get; set; }

        [JsonProperty("Congregation_ID")]
        public int? CongregationId { get; set; }

        [JsonProperty("Household_ID")]
        public int HouseholdId { get; set; }

        [JsonProperty("Statement_Frequency_ID")]
        public int StatementFrequencyId { get; set; }

        [JsonProperty("Statement_Type_ID")]
        public int StatementTypeId { get; set; }

        [JsonProperty("Statement_Method_ID")]
        public int StatementMethodId { get; set; }

        [JsonProperty("Setup_Date")]
        public DateTime SetupDate { get; set; }

        [JsonProperty("Processor_ID")]
        public string ProcessorId { get; set; }
    }
}
