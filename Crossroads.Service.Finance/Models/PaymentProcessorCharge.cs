using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class PaymentProcessorCharge
    {
        // TODO: Check the property name and type needed here
        [JsonProperty("payment_id")]
        public int PropertyId { get; set; }
    }
}
