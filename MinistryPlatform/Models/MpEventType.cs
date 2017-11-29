using System;
using System.Collections.Generic;
using System.Text;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Event_Types")]
    public class MpEventType
    {
        [JsonProperty(PropertyName = "Event_Type_ID")]
        public int EventTypeId { get; set; }

        [JsonProperty(PropertyName = "Event_Type")]
        public string EventType { get; set; }
    }
}
