using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;
using System;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Groups")]
    public class MpGroup
    {
        [JsonProperty(PropertyName = "Group_ID")]
        public int GroupId { get; set; }

        [JsonProperty(PropertyName = "Group_Name")]
        public string GroupName { get; set; }

    }
}
