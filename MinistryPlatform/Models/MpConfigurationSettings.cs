using System.Text.Json.Serialization;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "dp_Configuration_Settings")]
    public class MpConfigurationSettings
    {
        [JsonProperty("Configuration_Setting_ID")]
        public int ConfigurationSettingId { get; set; }

        [JsonProperty("Application_Code")] 
        public string ApplicationCode { get; set; }

        [JsonProperty("Key_Name")] 
        public string KeyName { get; set; }

        [JsonProperty("Value")] 
        public string Value { get; set; }

        [JsonProperty("Description")] 
        public string Description { get; set; }
    }
}