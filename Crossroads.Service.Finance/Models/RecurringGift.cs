using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class RecurringGiftDto
    {
        [JsonProperty("recurringGiftId")]
        public int RecurringGiftId { get; set; }

        [JsonProperty("donorId")]
        public int DonorId { get; set; }
    }
}