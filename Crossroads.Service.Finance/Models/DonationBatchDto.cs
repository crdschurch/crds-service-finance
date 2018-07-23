using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class DonationBatchDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("batchName")]
        public string BatchName { get; set; }

        [JsonProperty("setupDateTime")]
        public DateTime SetupDateTime { get; set; }

        [JsonProperty("batchTotalAmount")]
        public decimal BatchTotalAmount { get; set; }

        [JsonProperty("itemCount")]
        public int ItemCount { get; set; }

        [JsonProperty("batchEntryType")]
        public int BatchEntryType { get; set; }

        [JsonProperty("depositId")]
        public int? DepositId { get; set; }

        [JsonProperty("finalizedDateTime")]
        public DateTime FinalizedDateTime { get; set; }

        #region Donations property and accessor
        [JsonIgnore]
        private readonly List<DonationDto> _donations = new List<DonationDto>();
        [JsonProperty("donations")]
        public List<DonationDto> Donations { get { return (_donations); } }
        #endregion

        [JsonProperty("processorTransferIds")]
        public string ProcessorTransferId { get; set; }
    }
}