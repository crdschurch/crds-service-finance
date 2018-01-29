using System;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Programs")]
    public class MpProgram
    {
        [JsonProperty("Program_ID")]
        public int ProgramId { get; set; }

        [JsonProperty("Program_Name")]
        public string ProgramName { get; set; }
    }
}
