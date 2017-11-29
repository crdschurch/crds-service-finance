using System;
using System.Collections.Generic;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Opportunities")]
    public class MpOpportunity
    {
        public MpOpportunity()
        {
            Confirmed = new List<MpResponse>();
        }

        [JsonProperty(PropertyName = "Opportunity_ID")]
        public int OpportunityId { get; set; }

        [JsonProperty(PropertyName = "Opportunity_Title")]
        public string OpportunityTitle { get; set; }

        [JsonProperty(PropertyName = "Group_ID")]
        public int GroupId { get; set; }

        [JsonProperty(PropertyName = "Shift_Start")]
        public DateTime ShiftStart { get; set; }

        [JsonProperty(PropertyName = "Shift_End")]
        public DateTime ShiftEnd { get; set; }

        [JsonProperty(PropertyName = "Room")]
        public string Room { get; set; }

        [JsonProperty(PropertyName = "Event_Start_Date")]
        public DateTime? EventStartDate { get; set; }

        [JsonProperty(PropertyName = "Event_ID")]
        public int EventId { get; set; }

        [JsonProperty(PropertyName = "Event")]
        public MpEvent Event { get; set; }

        [JsonProperty(PropertyName = "Confirmed")]
        public List<MpResponse> Confirmed { get; set; }
    }
}
