using System;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Responses")]
    public class MpResponse
    {
        [JsonProperty(PropertyName = "Response_ID")]
        public int ResponseId { get; set; }

        [JsonProperty(PropertyName = "Response_Date")]
        public DateTime ResponseDate { get; set; }

        [JsonProperty(PropertyName = "Opportunity_ID")]
        public int OpportunityId { get; set; }

        [JsonProperty(PropertyName = "Response_Result_ID")]
        public int ResponseResultId { get; set; }

        [JsonProperty(PropertyName = "Event_ID")]
        public int EventId { get; set; }

        [JsonProperty(PropertyName = "Nickname")]
        public string Nickname { get; set; }

        [JsonProperty(PropertyName = "Last_Name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "Participant_ID")]
        public int ParticipantId { get; set; }

        [JsonProperty(PropertyName = "Event_Start_Date")]
        public DateTime EventStartDate { get; set; }

        [JsonProperty(PropertyName = "Closed")]
        public bool Closed { get; set; }

        [JsonProperty(PropertyName = "Domain_ID")]
        public int DomainId { get; set; }
    }
}
