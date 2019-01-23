using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;
using System;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "dp_Users")]
    public class MpUser
    {
        [JsonProperty(PropertyName = "User_ID")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "Contact_ID")]
        public int ContactId { get; set; }

        [JsonProperty(PropertyName = "User_Email")]
        public string UserEmail { get; set; }
    }
}
