using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Crossroads.Service.Finance.Models
{
    public class DonationDto
    {
        [JsonProperty("donation_id")]
        public int DonationId { get; set; }

        [JsonProperty("amount")]
        public string DonationAmt { get; set; }

        [JsonProperty("status_id"), JsonConverter(typeof(StringEnumConverter))]
        public int DonationStatusId { get; set; }

        [JsonProperty("date_status_date")]
        public DateTime DonationStatusDate { get; set; }

        [JsonProperty(PropertyName = "batch_id", NullValueHandling = NullValueHandling.Ignore)]
        public int? BatchId { get; set; }

        [JsonProperty("transaction_code")]
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