using System;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class ContactRelationship
    {
        [JsonProperty(PropertyName = "contactRelationshipId")]
        public int ContactRelationshipId { get; set; }

        [JsonProperty(PropertyName = "contactId")]
        public int ContactId { get; set; }

        [JsonProperty(PropertyName = "relationshipId")]
        public int RelationshipId { get; set; }

        [JsonProperty(PropertyName = "relatedContactId")]
        public int RelatedContactId { get; set; }

        [JsonProperty(PropertyName = "startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty(PropertyName = "endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }
    }
}
