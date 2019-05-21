using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;
using System;


namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Donation_Distributions")]
    public class MpDonationDistribution
    {
        [JsonProperty("Donation_Distribution_ID")]
        public int DonationDistributionId { get; set; }

        [JsonProperty("Donation_ID")]
        public int DonationId { get; set; }

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [JsonProperty("Pledge_ID")]
        public int? PledgeId { get; set; }

        [JsonProperty("Congregation_ID")]
        public int? CongregationId { get; set; }
    }
}
