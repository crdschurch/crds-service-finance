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
        public decimal DonationAmt { get; set; }

        [JsonProperty("Donation_Status_ID")]
        public int DonationStatusId { get; set; }

        [JsonProperty("Donation_Status_Date")]
        public DateTime DonationStatusDate { get; set; }

        [JsonProperty("Batch_ID")]
        public int? BatchId { get; set; }

        [JsonProperty("Transaction_Code")]
        public string TransactionCode { get; set; }

        [JsonProperty("Subscription_Code")]
        public string SubscriptionCode { get; set; }

        [JsonProperty("Is_Recurring_Gift")]
        public bool? IsRecurringGift { get; set; }

        [JsonProperty("Recurring_Gift_ID")]
        public int? RecurringGiftId { get; set; }
    }
}
