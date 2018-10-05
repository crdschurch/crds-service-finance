using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;
using System;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Pledges")]
    public class MpPledge
    {
        [JsonProperty(PropertyName = "Pledge_ID")]
        public int PledgeId { get; set; }

        [JsonProperty(PropertyName = "Pledge_Campaign_ID")]
        public int PledgeCampaignId { get; set; }

        [JsonProperty(PropertyName = "Pledge_Status_ID")]
        public int PledgeStatusId { get; set; }

        [JsonProperty(PropertyName = "Campaign_Name")]
        public string CampaignName { get; set; }

        [JsonProperty(PropertyName = "Total_Pledge")]
        public decimal PledgeTotal { get; set; }

        // total amount pledged so far
        [JsonProperty(PropertyName = "Total_Pledge_Donations")]
        public decimal? PledgeDonationsTotal { get; set; }

        [JsonProperty(PropertyName = "Campaign_Start_Date")]
        public DateTime? CampaignStartDate { get; set; }

        [JsonProperty(PropertyName = "Campaign_End_Date")]
        public DateTime? CampaignEndDate { get; set; }

        [JsonProperty(PropertyName = "First_Installment_Date")]
        public DateTime? FirstInstallmentDate { get; set; }
    }
}
