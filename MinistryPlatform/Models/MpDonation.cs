using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
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
        public int DonationAmt { get; set; }

        [JsonProperty("Donation_Date")]
        public DateTime DonationDate { get; set; }

        [JsonProperty("Payment_Type_ID")]
        public int PaymentTypeId { get; set; }

        [JsonProperty("Item_Number")]
        public string ItemNumber { get; set; }

        [JsonIgnore]
        public string DonationNotes { get; set; }

        [JsonProperty("Donation_Status_ID")]
        public int DonationStatusId { get; set; }

        [JsonProperty("Donation_Status_Date")]
        public DateTime DonationStatusDate { get; set; }

        [JsonProperty("Batch_ID")]
        public int? BatchId { get; set; }

        [JsonProperty("Transaction_Code")]
        public string TransactionCode { get; set; }

        [JsonIgnore]
        public bool IncludeOnGivingHistory { get; set; }

        [JsonIgnore]
        public bool IncludeOnPrintedStatement { get; set; }

        [JsonProperty("Is_Recurring_Gift")]
        public bool IsRecurringGift { get; set; }

        [JsonIgnore]
        public string AccountingCompanyName { get; set; }

        [JsonIgnore]
        public bool AccountingCompanyIncludeOnPrintedStatement { get; set; }

        #region Distributions property
        private readonly List<MpDonationDistribution> _distributions = new List<MpDonationDistribution>();
        public List<MpDonationDistribution> Distributions { get { return (_distributions); } }
        #endregion

        public MpDonation()
        {
            IncludeOnGivingHistory = true;
            IncludeOnPrintedStatement = false;
        }
    }
}
