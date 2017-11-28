using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;
using System;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Events")]
    public class MpEvent
    {

        [JsonProperty(PropertyName = "Event_ID")]
        public int EventId { get; set; }

        [JsonProperty(PropertyName = "Event_Title")]
        public string EventTitle { get; set; }

        [JsonProperty(PropertyName = "Event_Start_Date")]
        public DateTime EventStartDate { get; set; }

        [JsonProperty(PropertyName = "Event_End_Date")]
        public DateTime EventEndDate { get; set; }

        [JsonProperty(PropertyName = "Event_Type_ID")]
        public int EventTypeId { get; set; }

        [JsonProperty(PropertyName = "Congregation_Name")]
        public string CongregationName { get; set; }

        [JsonProperty(PropertyName = "Congregation_ID")]
        public int CongregationId { get; set; }

        [JsonProperty(PropertyName = "Location_ID")]
        public int LocationId { get; set; }

        [JsonProperty(PropertyName = "Program_ID")]
        public int ProgramId { get; set; }

        [JsonProperty(PropertyName = "Cancelled")]
        public bool Cancelled { get; set; }

    }
}
