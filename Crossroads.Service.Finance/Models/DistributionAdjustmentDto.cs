using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class DistributionAdjustmentDto
    {
        [JsonProperty("distributionAdjustmentId")]
        public int DistributionAdjustmentId { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("donationDate")]
        public DateTime DonationDate { get; set; }

        [JsonProperty("processedDate")]
        public DateTime? ProcessedDate { get; set; }

        [JsonProperty("glAccountNumber")]
        public string GLAccountNumber { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("adjustment")]
        public string Adjustment { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("donationDistributionId")]
        public int? DonationDistributionID { get; set; }
    }
}
