using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;
using System;


namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Donation_Distributions")]
    public class MpDonationDistribution
    {
        [JsonProperty("Donation_Distribution_ID")]
        public int DonationId { get; set; }

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [JsonProperty("Pledge_ID")]
        public int PledgeId { get; set; }
    }
}
