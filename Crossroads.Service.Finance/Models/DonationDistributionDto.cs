using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Models
{
    public class DonationDistributionDto
    {
        [JsonProperty("program_name")]
        public string ProgramName { get; set; }
        [JsonProperty("amount")]
        public int Amount { get; set; }
    }
}
