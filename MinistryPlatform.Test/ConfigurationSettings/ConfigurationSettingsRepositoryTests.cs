using System;
using System.Threading.Tasks;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Models;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MinistryPlatform.Test.ConfigurationSettings
{
    public class ConfigurationSettingsRepositoryTests
    {
        private readonly Mock<IApiUserRepository> _apiUserRepository;
        private readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        private readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        private readonly Mock<IMinistryPlatformRestRequestAsync> _request;

        private readonly ConfigurationSettingsRepository _fixture;
        
        private readonly string token = "sdfjiodsaf";

        public ConfigurationSettingsRepositoryTests()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _restRequest = new Mock<IMinistryPlatformRestRequestBuilder>(MockBehavior.Strict);
            _restRequestBuilder = new Mock<IMinistryPlatformRestRequestBuilderFactory>(MockBehavior.Strict);
            _request = new Mock<IMinistryPlatformRestRequestAsync>(MockBehavior.Strict);

            _apiUserRepository.Setup(m => m.GetApiClientTokenAsync("CRDS.Service.Finance")).ReturnsAsync(token);

            _fixture = new ConfigurationSettingsRepository(_apiUserRepository.Object, _restRequestBuilder.Object);
        }

        [Fact]
        public async Task ShouldUpdateConfigSettings()
        {
            // Arrange
            var configurationId = 300;
            var value = DateTime.Now.ToString();
            var updateObject = new JObject
            {
                {"Configuration_Setting_ID", configurationId},
                {"Value", value}
            };

            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);
            _request.Setup(m => m.Update(It.Is<JObject>(v => 
                v["Configuration_Setting_ID"].ToString() == configurationId.ToString()
                && v["Value"].ToString() == value), "dp_Configuration_Settings", false)).ReturnsAsync(new JObject());
            
            
            // Act 
            await _fixture.UpdateValue(configurationId, value);
            
            // Assert
            _apiUserRepository.Verify(m => m.GetApiClientTokenAsync("CRDS.Service.Finance"), Times.Once);
            _request.Verify(m => m.Update(It.Is<JObject>(v => 
                                                          v["Configuration_Setting_ID"].ToString() == configurationId.ToString()
                                                          && v["Value"].ToString() == value), "dp_Configuration_Settings", false), Times.Once);

        }
        
        [Fact]
        public async Task ShouldCreateConfigSettings()
        {
            // Arrange
            var value = DateTime.Now.ToString();
            var config = new MpConfigurationSettings
            {
               ConfigurationSettingId = 300,
               Description = "Some description",
               ApplicationCode = "CRDS-FINANCE",
               Value = value,
               KeyName = "DonationSyncUpTo"
            };

            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);
            _request.Setup(m => m.Create(config, null)).ReturnsAsync(new MpConfigurationSettings());
            
            
            // Act 
            await _fixture.CreateConfigurationSettings(config);
            
            // Assert
            _apiUserRepository.Verify(m => m.GetApiClientTokenAsync("CRDS.Service.Finance"), Times.Once);
            _request.Verify(m => m.Create(config, null), Times.Once);

        }
    }
}