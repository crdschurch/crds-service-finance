using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;
using System;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Contacts")]
    public class MpContact
    {
        [JsonProperty(PropertyName = "Contact_ID")]
        public int ContactId { get; set; }

        [JsonProperty(PropertyName = "Household_ID")]
        public int HouseholdId { get; set; }
    }
}
