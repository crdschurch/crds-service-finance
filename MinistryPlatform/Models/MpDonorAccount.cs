using System;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Donor_Accounts")]
    public class MpDonorAccount
    {
        [JsonProperty("Donor_Account_ID")]
        public int DonorAccountId { get; set; }

        [JsonProperty("Institution_Name")]
        public string InstitutionName { get; set; }

        [JsonProperty("Account_Number")]
        public string AccountNumber { get; set; }

        [JsonProperty("Routing_Number")]
        public string RoutingNumber { get; set; }
    }
}
