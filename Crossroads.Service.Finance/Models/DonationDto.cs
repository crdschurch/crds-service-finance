using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Crossroads.Service.Finance.Models
{
    public class DonationDto
    {
        [JsonProperty("donationId")]
        public int DonationId { get; set; }

        [JsonProperty("amount")]
        public decimal DonationAmt { get; set; }

        [JsonProperty("statusId")]
        public int DonationStatusId { get; set; }

        [JsonProperty("dateStatusDate")]
        public DateTime DonationStatusDate { get; set; }

        [JsonProperty(PropertyName = "batchId", NullValueHandling = NullValueHandling.Ignore)]
        public int? BatchId { get; set; }

        [JsonProperty("transactionCode")]
        public string TransactionCode { get; set; }
    }

    public enum DonationStatus
    {
        [EnumMember(Value = "Pending")]
        Pending = 1,
        [EnumMember(Value = "Deposited")]
        Deposited = 2,
        [EnumMember(Value = "Declined")]
        Declined = 3,
        [EnumMember(Value = "Succeeded")]
        Succeeded = 4,
        [EnumMember(Value = "Refunded")]
        Refunded = 5
    }
}