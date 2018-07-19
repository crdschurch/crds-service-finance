using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class RecurringGiftDto
    {
        [JsonProperty("recurringGiftId")]
        public int RecurringGiftId { get; set; }

        [JsonProperty("contactId")]
        public int ContactId { get; set; }

        [JsonProperty("donorId")]
        public int DonorId { get; set; }

        [JsonProperty("frequencyId")]
        public int FrequencyId { get; set; }

        [JsonProperty("dayOfMonth")]
        public int? DayOfMonth { get; set; }

        [JsonProperty("dayOfWeekId")]
        public int? DayOfWeek { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("programId")]
        public int ProgramId { get; set; }

        [JsonProperty("programName")]
        public string ProgramName { get; set; }

        [JsonProperty("subscriptionId")]
        public string SubscriptionId { get; set; }

        [JsonProperty("sourceUrl")]
        public string SourceUrl { get; set; }

        [JsonProperty("predefinedAmount")]
        public decimal? PredefinedAmount { get; set; }

        [JsonProperty("vendorDetailUrl")]
        public string VendorDetailUrl { get; set; }

        [JsonProperty("recurringGiftStatusId")]
        public int RecurringGiftStatusId { get; set; }
    }
}