using Newtonsoft.Json;
using System;

namespace Crossroads.Service.Finance.Models
{
    /// <summary>
    /// The data needed to view all Pledges in MinistryPlatform.
    /// </summary>
    public class PledgeDto
    {
        [JsonProperty(PropertyName = "pledgeId")]
        public int PledgeId { get; set; }

        [JsonProperty(PropertyName = "campaignId")]
        public int PledgeCampaignId { get; set; }

        [JsonProperty(PropertyName = "statusId")]
        public int PledgeStatusId { get; set; }

        [JsonProperty(PropertyName = "campaignName")]
        public string CampaignName { get; set; }

        [JsonProperty(PropertyName = "total")]
        public decimal PledgeTotal { get; set; }

        [JsonProperty(PropertyName = "donations")]
        public decimal PledgeDonationsTotal { get; set; }

        [JsonProperty(PropertyName = "campaignStartDate")]
        public DateTime? CampaignStartDate { get; set; }

        [JsonProperty(PropertyName = "campaignEndDate")]
        public DateTime? CampaignEndDate { get; set; }
    }


}