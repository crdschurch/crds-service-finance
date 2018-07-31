using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Donor_Accounts")]
    public class MpDonorAccount
    {
        [JsonProperty("Donor_Account_ID")]
        public int DonorAccountId { get; set; }

        [JsonProperty("Donor_ID")]
        public int DonorId { get; set; }

        [JsonProperty("Non-Assignable")]
        public bool NonAssignable { get; set; }

        [JsonProperty("Domain_ID")]
        public int DomainId { get; set; }

        [JsonProperty("Account_Type_ID")]
        public int AccountTypeId { get; set; }

        [JsonProperty("Closed")]
        public bool Closed { get; set; }

        [JsonProperty("Institution_Name")]
        public string InstitutionName { get; set; }

        [JsonProperty("Account_Number")]
        public string AccountNumber { get; set; }

        [JsonProperty("Routing_Number")]
        public string RoutingNumber { get; set; }

        [JsonProperty("Processor_ID")]
        public string ProcessorId { get; set; }

        [JsonProperty("Processor_Type_ID")]
        public int ProcessorTypeId { get; set; }
    }
}
