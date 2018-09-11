using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpayPaymentsDto: PushpayResponseBaseDto
    {
        [JsonProperty("items")]
        public List<PushpayPaymentDto> Items { get; set; }

    }
}
