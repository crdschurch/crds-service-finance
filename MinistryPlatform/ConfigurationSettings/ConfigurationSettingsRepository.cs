using System.Threading.Tasks;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform
{
    public class ConfigurationSettingsRepository : IConfigurationSettingsRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRequestBuilderFactory _builder;

        public ConfigurationSettingsRepository(IApiUserRepository apiUserRepository,
            IMinistryPlatformRestRequestBuilderFactory ministryPlatformRestRequestBuilderFactory)
        {
            _apiUserRepository = apiUserRepository;
            _builder = ministryPlatformRestRequestBuilderFactory;
        }

        public async Task UpdateValue(int configurationSettingsId, string value)
        {
            var token = await _apiUserRepository.GetApiClientTokenAsync("CRDS.Service.Finance");
            var updateObject = new JObject
            {
                {"Configuration_Setting_ID", configurationSettingsId},
                {"Value", value}
            };

            await _builder.NewRequestBuilder().WithAuthenticationToken(token)
                .BuildAsync().Update(updateObject, "dp_Configuration_Settings");
        }

        public async Task CreateConfigurationSettings(MpConfigurationSettings configurationSettings)
        {
            var token = await _apiUserRepository.GetApiClientTokenAsync("CRDS.Service.Finance");

            await _builder.NewRequestBuilder().WithAuthenticationToken(token)
                .BuildAsync().Create(configurationSettings);
        }
    }
}