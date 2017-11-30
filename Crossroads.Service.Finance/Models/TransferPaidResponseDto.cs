using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class TransferPaidResponseDto : PaymentEventResponseDto
    {
        [JsonProperty("transaction_count")]
        public int TotalTransactionCount { get; set; }

        [JsonProperty("successful_updates")]
        public List<string> SuccessfulUpdates { get; } = new List<string>();

        [JsonProperty("failed_updates")]
        public List<KeyValuePair<string, string>> FailedUpdates { get; } = new List<KeyValuePair<string, string>>();

        [JsonProperty("donation_batch")]
        public List<DonationBatchDto> Batch { get; } = new List<DonationBatchDto>();

        [JsonProperty("deposit")]
        public DepositDto Deposit;
    }
}
