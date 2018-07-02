using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class RecurringGiftDto
    {
        [JsonProperty("recurringGiftId")]
        public int RecurringGiftId { get; set; }

        [JsonProperty("Contact_ID")]
        public int ContactId { get; set; }

        [JsonProperty("Donor_ID")]
        public int DonorId { get; set; }

        [JsonProperty("Frequency_ID")]
        public int FrequencyId { get; set; }

        [JsonProperty("Day_Of_Month")]
        public int? DayOfMonth { get; set; }

        [JsonProperty("Day_Of_Week_ID")]
        public int? DayOfWeek { get; set; }

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [JsonProperty("Program_ID")]
        public int ProgramId { get; set; }

        [JsonProperty("Subscription_ID")]
        public string SubscriptionId { get; set; }

        [JsonProperty("Source_Url")]
        public string SourceUrl { get; set; }

        [JsonProperty("Predefined_Amount")]
        public decimal? PredefinedAmount { get; set; }

        [JsonProperty("Vendor_Detail_Url")]
        public string VendorDetailUrl { get; set; }
    }
}