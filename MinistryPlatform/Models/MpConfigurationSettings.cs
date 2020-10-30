using System.Text.Json.Serialization;
using Crossroads.Web.Common.MinistryPlatform;

namespace MinistryPlatform.Models
{
    [MpRestApiTable(Name = "dp_Configuration_Settings")]
    public class MpConfigurationSettings
    {
        [JsonPropertyName("Configuration_Setting_ID")]
        public int ConfigurationSettingId { get; set; }

        [JsonPropertyName("Application_Code")] 
        public string ApplicationCode { get; set; }

        [JsonPropertyName("Key_Name")] 
        public string KeyName { get; set; }

        [JsonPropertyName("Value")] 
        public string Value { get; set; }

        [JsonPropertyName("Description")] 
        public string Description { get; set; }
    }
}