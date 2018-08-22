using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class DonorAccountDto
    {
        [JsonProperty("donorAccountId")]
        public int DonorAccountId { get; set; }

        [JsonProperty("donorId")]
        public int DonorId { get; set; }

        [JsonProperty("nonAssignable")]
        public bool NonAssignable { get; set; }

        [JsonProperty("accountTypeId")]
        public int AccountTypeId { get; set; }

        [JsonProperty("closed")]
        public bool Closed { get; set; }

        [JsonProperty("institutionName")]
        public string InstitutionName { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("routingNumber")]
        public string RoutingNumber { get; set; }

        [JsonProperty("processorId")]
        public string ProcessorId { get; set; }

        [JsonProperty("processorTypeId")]
        public int ProcessorTypeId { get; set; }
    }
}
