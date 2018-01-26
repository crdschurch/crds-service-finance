using Crossroads.Web.Common.MinistryPlatform;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Recurring_Gifts")]
    public class MpRecurringGift
    {
        //[JsonProperty("Recurring_Gift_ID")]
        //public int RecurringGiftId { get; set; }

        [JsonProperty("Donor_ID")]
        public int DonorId { get; set; }

        // [JsonProperty("Donor_Account_ID")]
        // public int DonorAccountId { get; set; }

        [JsonProperty("Frequency_ID")]
        public int FrequencyId { get; set; }

        [JsonProperty("Day_Of_Month")]
        public int DayOfMonth { get; set; }

        [JsonProperty("Day_Of_Week_ID")]
        public int DayOfWeek { get; set; }

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [JsonProperty("End_Date")]
        public DateTime EndDate { get; set; }

        [JsonProperty("Program_ID")]
        public int ProgramId { get; set; }

        [JsonProperty("Congregation_ID")]
        public int CongregationId { get; set; }

        [JsonProperty("Domain_ID")]
        public int DomainId { get; set; }

        // [JsonProperty("Subscription_ID")]
        // public int SubscriptionId { get; set; }

        // [JsonProperty("Source_Url")]
        // public string SourceUrl { get; set; }

        // [JsonProperty("Predefined_Amount")]
        // public decimal PredefinedAmount { get; set; }
    }
}
