using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crossroads.Service.Serve.Models
{
    public class OpportunityDto
    {
        public OpportunityDto()
        {
            Confirmed = new List<ResponseDto>();
        }

        [JsonProperty(PropertyName = "opportunityId")]
        public int OpportunityId { get; set; }

        [JsonProperty(PropertyName = "opportunityTitle")]
        public string OpportunityTitle { get; set; }

        [JsonProperty(PropertyName = "groupId")]
        public int GroupId { get; set; }

        [JsonProperty(PropertyName = "shiftStart")]
        public DateTime ShiftStart { get; set; }

        [JsonProperty(PropertyName = "shiftEnd")]
        public DateTime ShiftEnd { get; set; }

        [JsonProperty(PropertyName = "room")]
        public string Room { get; set; }

        [JsonProperty(PropertyName = "eventStartDate")]
        public DateTime? EventStartDate { get; set; }

        [JsonProperty(PropertyName = "eventId")]
        public int EventId { get; set; }

        [JsonProperty(PropertyName = "confirmed")]
        public List<ResponseDto> Confirmed { get; set; }
    }
}
