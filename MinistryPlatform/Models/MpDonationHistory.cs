using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;
using System;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Donation_Distributions")]
    public class MpDonationHistory
    {
        [JsonProperty("Donation_ID")]
        public int DonationId { get; set; }

        [JsonProperty("Donation_Distribution_ID")]
        public int DonationDistributionId { get; set; }

        [JsonProperty("Donation_Status_Date")]
        public DateTime DonationStatusDate { get; set; }

        [JsonProperty("Program_Name")]
        public string ProgramName { get; set; }

        [JsonProperty("Donation_Status_ID")]
        public int DonationStatusId { get; set; }

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [JsonProperty("Donation_Date")]
        public DateTime DonationDate { get; set; }

        [JsonProperty("Donation_Status")]
        public string DonationStatus { get; set; }

        [JsonProperty("Account_Number")]
        public string AccountNumber { get; set; }

        [JsonProperty("Institution_Name")]
        public string InstitutionName { get; set; }

        [JsonProperty("Routing_Number")]
        public string RoutingNumber { get; set; }

        [JsonProperty("Account_Type")]
        public string AccountType { get; set; }

        [JsonProperty("Processor_Type")]
        public string ProcessorType { get; set; }

        [JsonProperty("Payment_Type")]
        public string PaymentType { get; set; }

    }
}
