using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class PaymentsDto
    {
        // TODO: Check the property name and type needed here
        [JsonProperty("items")]
        public List<PaymentProcessorChargeDto> payments { get; set; }
    }
}
