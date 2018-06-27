using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Crossroads.Service.Finance.Models
{
    /// <summary>
    /// The data needed to view all Pledges in MinistryPlatform.
    /// </summary>
    public class PledgeDto
    {
        /// <summary>
        /// The ID of the MP pledge. 
        /// </summary>
        [JsonProperty(PropertyName = "pledge_id")]
        [Required]
        public int PledgeId { get; set; }

        /// <summary>
        /// The ID of the MP pledge. 
        /// </summary>
        [JsonProperty(PropertyName = "pledge_campaign_id")]
        [Required]
        public int PledgeCampaignId { get; set; }

        /// <summary>
        /// This is a name of the pledge campaign.
        /// </summary>
        [JsonProperty(PropertyName = "pledge_campaign")]
        [Required]
        public string PledgeCampaign { get; set; }

        /// <summary>
        /// The status of the pledge.
        /// </summary>
        [JsonProperty(PropertyName = "pledge_status")]
        [Required]
        public string PledgeStatus { get; set; }

        /// <summary>
        /// The date the campaign began.
        /// </summary>
        [JsonProperty(PropertyName = "campaign_start_date")]
        [Required]
        public string CampaignStartDate { get; set; }

        /// <summary>
        /// The date the pledge ended.
        /// </summary>
        [JsonProperty(PropertyName = "campaign_end_date")]
        [Required]

        public string CampaignEndDate { get; set; }

        /// <summary>
        /// The total amount pledged.
        /// </summary>
        [JsonProperty(PropertyName = "total_pledge")]
        [Required]
        public decimal TotalPledge { get; set; }

        /// <summary>
        /// The total amount donated toward the pledge.
        /// </summary>
        [JsonProperty(PropertyName = "pledge_donations")]
        [Required]
        public decimal PledgeDonations { get; set; }

        /// <summary>
        /// The campaign type ID of the pledge.
        /// </summary>
        [JsonProperty(PropertyName = "campaign_type_id")]
        public int CampaignTypeId { get; set; }

        /// <summary>
        /// The campaign type name of the pledge.
        /// </summary>
        [JsonProperty(PropertyName = "campaign_type_name")]
        public string CampaignTypeName { get; set; }
    }


}