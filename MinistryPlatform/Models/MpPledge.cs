using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;
using System;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Pledges")]
    public class MpPledge
    {
        [JsonProperty(PropertyName = "pledge_id")]
        public int PledgeId { get; set; }

        [JsonProperty(PropertyName = "pledge_campaign_id")]
        public int PledgeCampaignId { get; set; }

        [JsonProperty(PropertyName = "pledge_campaign")]
        public string PledgeCampaign { get; set; }

        [JsonProperty(PropertyName = "pledge_status")]
        public string PledgeStatus { get; set; }

        [JsonProperty(PropertyName = "campaign_start_date")]
        public string CampaignStartDate { get; set; }

        [JsonProperty(PropertyName = "campaign_end_date")]
        public string CampaignEndDate { get; set; }

        [JsonProperty(PropertyName = "total_pledge")]
        public decimal TotalPledge { get; set; }

        [JsonProperty(PropertyName = "pledge_donations")]
        public decimal PledgeDonations { get; set; }

        [JsonProperty(PropertyName = "campaign_type_id")]
        public int CampaignTypeId { get; set; }

        [JsonProperty(PropertyName = "campaign_type_name")]
        public string CampaignTypeName { get; set; }

    }
}
