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

        [JsonProperty("batch_name")]
        public string BatchName { get; set; }

        [JsonProperty("setup_date_time")]
        public DateTime SetupDateTime { get; set; }

        [JsonProperty("batch_total_amount")]
        public decimal BatchTotalAmount { get; set; }

        [JsonProperty("item_count")]
        public int ItemCount { get; set; }

        [JsonProperty("batch_entry_type")]
        public int BatchEntryType { get; set; }

        [JsonProperty("deposit_id")]
        public int? DepositId { get; set; }

        [JsonProperty("finalized_date_time")]
        public DateTime FinalizedDateTime { get; set; }

        #region Donations property and accessor
        [JsonIgnore]
        private readonly List<DonationDto> _donations = new List<DonationDto>();
        [JsonProperty("donations")]
        public List<DonationDto> Donations { get { return (_donations); } }
        #endregion

        #region Payments property and accessor
        [JsonIgnore]
        private readonly List<PaymentDto> _payments = new List<PaymentDto>();
        [JsonProperty("payments")]
        public List<PaymentDto> Payments { get { return (_payments); } }
        #endregion

        [JsonProperty("processor_transfer_id")]
        public string ProcessorTransferId { get; set; }

        [JsonProperty("program_id")]
        public int ProgramId { get; set; }

        [JsonProperty("batch_fee_total")]
        public decimal BatchFeeTotal { get; set; }
    }
}
