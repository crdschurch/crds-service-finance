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

        [JsonProperty(PropertyName = "Date_of_Birth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty(PropertyName = "Participant_Record")]
        public int ParticipantId { get; set; }

        [JsonProperty(PropertyName = "Household_ID")]
        public int? HouseholdId { get; set; }

        [JsonProperty(PropertyName = "Email_Address")]
        public string EmailAddress { get; set; }

        [JsonProperty(PropertyName = "Mobile_Phone")]
        public string MobilePhone { get; set; }

        [JsonProperty(PropertyName = "Nickname")]
        public string Nickname { get; set; }

        [JsonProperty(PropertyName = "First_Name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "Last_Name")]
        public string LastName { get; set; }
    }
}
