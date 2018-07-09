using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Households")]
    public class MpHousehold
    {
        [JsonProperty("Household_ID")]
        public int HouseholdId { get; set; }

        [JsonProperty("Congregation_ID")]
        public int CongregationId { get; set; }
    }
}
