using System.Threading.Tasks;
using MinistryPlatform.Models;

namespace MinistryPlatform
{
    public interface IConfigurationSettingsRepository
    {
        Task UpdateValue(int configurationSettingsId, string value);
        Task CreateConfigurationSettings(MpConfigurationSettings configurationSettings);
    }
}