using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;
using System;


namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Donations")]
    public class MpDonation
    {
        [JsonProperty("Donation_ID")]
        public int DonationId { get; set; }

        [JsonProperty("Contact_ID")]
        public int ContactId { get; set; }

        [JsonProperty("Donation_Amount")]
        public double DonationAmt { get; set; }

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
