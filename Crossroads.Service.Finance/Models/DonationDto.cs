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
        [JsonProperty(PropertyName = "program_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ProgramId { get; set; }
        [JsonProperty("amount")]
        public Decimal Amount { get; set; }
        [JsonIgnore]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "batch_id", NullValueHandling = NullValueHandling.Ignore)]
        public int? BatchId { get; set; }

        [JsonProperty("status"), JsonConverter(typeof(StringEnumConverter))]
        public DonationStatus Status { get; set; }

        [JsonProperty(PropertyName = "include_on_giving_history")]
        public bool IncludeOnGivingHistory { get; set; }

        [JsonProperty(PropertyName = "include_on_printed_statement")]
        public bool IncludeOnPrintedStatement { get; set; }

        [JsonProperty("date")]
        public DateTime DonationDate { get; set; }

        [JsonProperty("fee")]
        public decimal Fee { get; set; }

        [JsonProperty("payment_id")]
        public int PaymentId { get; set; }

        [JsonProperty("accounting_company_name")]
        public string AccountingCompanyName { get; set; }

        [JsonProperty("accounting_company_include_on_printed_statement")]
        public bool AccountingCompanyIncludeOnPrintedStatement { get; set; }

        [JsonProperty(PropertyName = "source", NullValueHandling = NullValueHandling.Ignore)]
        public DonationSourceDto Source { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("transaction_code")]
        public string TransactionCode { get; set; }

        #region Distributions Property
        [JsonIgnore]
        private readonly List<DonationDistributionDto> _distributions = new List<DonationDistributionDto>();
        [JsonProperty(PropertyName = "distributions", NullValueHandling = NullValueHandling.Ignore)]
        public List<DonationDistributionDto> Distributions { get { return (_distributions); } }
        #endregion

        public DonationDto()
        {
            Source = new DonationSourceDto();
        }
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
