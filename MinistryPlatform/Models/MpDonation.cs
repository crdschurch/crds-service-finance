using System;
using System.Collections.Generic;
using System.Text;

namespace MinistryPlatform.Models
{
    public class MpDonation
    {
        public int donationId { get; set; }
        public int donorId { get; set; }
        public int softCreditDonorId { get; set; }
        public string donorDisplayName { get; set; }
        public int donationAmt { get; set; }
        public DateTime donationDate { get; set; }
        public int paymentTypeId { get; set; }
        public string itemNumber { get; set; }
        public string donationNotes { get; set; }
        public int donationStatus { get; set; }
        public DateTime donationStatusDate { get; set; }
        public int? batchId { get; set; }
        public string transactionCode { get; set; }
        public bool IncludeOnGivingHistory { get; set; }
        public bool IncludeOnPrintedStatement { get; set; }
        public bool recurringGift { get; set; }
        public string AccountingCompanyName { get; set; }
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
