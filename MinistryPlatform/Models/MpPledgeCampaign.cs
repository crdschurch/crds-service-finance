using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Pledge_Campaigns")]
    public class MpPledgeCampaign
    {
        [JsonProperty(PropertyName = "Pledge_Campaign_ID")]
        public int PledgeCampaignId { get; set; }

        [JsonProperty(PropertyName = "Campaign_Name")]
        public string CampaignName { get; set; }

        [JsonProperty(PropertyName = "Pledge_Campaign_Type_ID")]
        public int PledgeCampaignTypeId { get; set; }
    }
}
