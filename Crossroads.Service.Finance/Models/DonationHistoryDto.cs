using Newtonsoft.Json;
using System;

namespace Crossroads.Service.Finance.Models
{
    public class DonationHistoryDto
    {
        [JsonProperty("donationId")]
        public int DonationId { get; set; }

        [JsonProperty("donationDistributionId")]
        public int DonationDistributionId { get; set; }

        [JsonProperty("donationStatusDate")]
        public DateTime DonationStatusDate { get; set; }

        [JsonProperty("programName")]
        public string ProgramName { get; set; }

        [JsonProperty("donationStatusId")]
        public int DonationStatusId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("donationDate")]
        public DateTime DonationDate { get; set; }

        [JsonProperty("donationStatus")]
        public string DonationStatus { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("institutionName")]
        public string InstitutionName { get; set; }

        [JsonProperty("paymentType")]
        public string PaymentType { get; set; }
    }
}
