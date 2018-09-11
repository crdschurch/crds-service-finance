using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pushpay.Models
{
    public class PushpaySettlementResponseDto: PushpayResponseBaseDto
    {
        [JsonProperty("items")]
        public new List<PushpaySettlementDto> items { get; set; }
    }
}
