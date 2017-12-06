using Crossroads.Web.Common.MinistryPlatform;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Donations")]
    public class MpDonation
    {
        [JsonProperty("Donation_ID")]
        public int DonationId { get; set; }

        [JsonProperty("Donor_ID")]
        public int DonorId { get; set; }

        //[JsonProperty("Donor_ID")]
        [JsonIgnore]
        public int SoftCreditDonorId { get; set; }

        [JsonIgnore]
        public string DonorDisplayName { get; set; }

        [JsonProperty("Donation_Amount")]
        public string DonationAmt { get; set; }

        [JsonProperty("Donation_Date")]
        public DateTime DonationDate { get; set; }

        [JsonProperty("Donation_Status_ID")]
        public int DonationStatusId { get; set; }

        [JsonProperty("Donation_Status_Date")]
        public DateTime DonationStatusDate { get; set; }

        [JsonProperty("Batch_ID")]
        public int? BatchId { get; set; }

        [JsonProperty("Transaction_Code")]
        public string TransactionCode { get; set; }
    }
}
