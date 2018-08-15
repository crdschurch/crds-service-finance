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

        [JsonProperty("donationStatusDate")]
        public DateTime DonationStatusDate { get; set; }

        [JsonProperty(PropertyName = "batchId", NullValueHandling = NullValueHandling.Ignore)]
        public int? BatchId { get; set; }

        // this is the numeric id that pushpay stores on our donation
        [JsonProperty("transactionCode")]
        public string TransactionCode { get; set; }

        // this is the alphanumeric id that we save to identify pushpay donations via api
        [JsonProperty("subscriptionCode")]
        public string SubscriptionCode { get; set; }

        [JsonProperty("isRecurringGift")]
        public bool? IsRecurringGift { get; set; }

        [JsonProperty("recurringGiftId")]
        public int? RecurringGiftId { get; set; }
        
        [JsonProperty("paymentTypeId")]
        public int PaymentTypeId { get; set; }
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