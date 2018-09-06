using System;
using System.Collections.Generic;
using System.Text;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Contacts")]
    public class MpContactAddress
    {
        [JsonProperty("Contact_ID")]
        public int ContactId { get; set; }

        [JsonProperty("Address_ID")]
        public int AddressId { get; set; }

        [JsonProperty("Address_Line_1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("Address_Line_2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("State/Region")]
        public string State { get; set; }

        [JsonProperty("Postal_Code")]
        public string PostalCode { get; set; }
    }
}
