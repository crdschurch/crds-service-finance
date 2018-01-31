using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayRecurringGiftDto
    {

        [JsonProperty("schedule")]
        public RecurringGiftSchedule Schedule { get; set; }

        [JsonProperty("status")]
        public string Status{ get; set; }

        [JsonProperty("amount")]
        public RecurringGiftAmount Amount { get; set; }

        [JsonProperty("payer")]
        public PushpayPayer Payer { get; set; }

        [JsonProperty("paymentMethodType")]
        public string PaymentMethodType { get; set; }

        [JsonProperty("paymentToken")]
        public string PaymentToken { get; set; }

        [JsonProperty("card")]
        public PushpayCard Card { get; set; }

        [JsonProperty("account")]
        public PushpayAccount Account { get; set; }

        [JsonProperty("fund")]
        public PushpayFund Fund { get; set; }

        [JsonProperty("links")]
        public PushpayLinksDto Links { get; set; }
    }

    public class RecurringGiftSchedule
    {
        [JsonProperty("frequency")]
        public string Frequency{ get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
    }

    public class RecurringGiftAmount
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class PushpayPayer
    {
        [JsonProperty("key")]
        public string Key{ get; set; }

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }
    }

    public class PushpayCard
    {
        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }
    }

    public class PushpayAccount
    {
        [JsonProperty("routingNumber")]
        public string RoutingNumber { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("bankName")]
        public string BankName { get; set; }

        [JsonProperty("accountName")]
        public string AccountName { get; set; }

        [JsonProperty("accountType")]
        public string AccountType { get; set; }
    }

    public class PushpayFund
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("taxDeductible")]
        public string TaxDeductible { get; set; }
    }
}
