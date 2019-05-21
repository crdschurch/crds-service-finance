using System;
using System.Collections.Generic;
using System.Text;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "Congregations")]
    public class MpCongregation
    {
        [JsonProperty(PropertyName = "Congregation_ID")]
        public int CongregationId { get; set; }

        [JsonProperty(PropertyName = "Congregation_Name")]
        public string CongregationName { get; set; }
    }
}
