using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class PaymentEventResponseDto
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
