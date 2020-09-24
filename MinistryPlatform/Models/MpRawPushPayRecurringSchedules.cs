using System;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "cr_RawPushpayRecurringSchedules")]
    public class MpRawPushPayRecurringSchedules
    {
        [JsonProperty(PropertyName = "RecurringGiftScheduleId")]
        public int RecurringGiftScheduleId { get; set; }

        [JsonProperty(PropertyName = "RawJson")]
        public string RawJson { get; set; }
        
        [JsonProperty(PropertyName = "IsProcessed")]
        public bool IsProcessed { get; set; }
        
        [JsonProperty(PropertyName = "TimeCreated")]
        public DateTime TimeCreated { get; set; }
    }
}