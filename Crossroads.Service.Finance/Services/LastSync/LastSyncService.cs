using System;
using System.Threading.Tasks;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform;
using MinistryPlatform.Models;

namespace Crossroads.Service.Finance.Services
{
    public class LastSyncService : ILastSyncService
    {
        private readonly IConfigurationSettingsRepository _configurationSettingsRepository;
        private readonly IConfigurationWrapper _configurationWrapper;

        public LastSyncService(IConfigurationSettingsRepository configurationSettingsRepository,
            IConfigurationWrapper configurationWrapper)
        {
            _configurationSettingsRepository = configurationSettingsRepository;
            _configurationWrapper = configurationWrapper;
        }

        public async Task<DateTime> GetLastDonationSyncTime()
        {
            var lastSync = await _configurationWrapper.GetMpConfigValueAsync("CRDS-FINANCE", "LastDonationSync");
            return string.IsNullOrWhiteSpace(lastSync) ? DateTime.Now.AddHours(-1) : DateTime.Parse(lastSync);
        }

        public async Task<DateTime> GetLastRecurringScheduleSyncTime()
        {
            var lastSync = await _configurationWrapper.GetMpConfigValueAsync("CRDS-FINANCE", "LastRecurringSync");
            return string.IsNullOrWhiteSpace(lastSync) ? DateTime.Now.AddHours(-1) : DateTime.Parse(lastSync);
        }

        public async Task UpdateDonationSyncTime(DateTime newSyncTime)
        {
            var configId =
                await _configurationSettingsRepository.GetConfigurationId("CRDS-FINANCE", "LastDonationSync");
            if (!configId.HasValue)
            {
                var newEntry = new MpConfigurationSettings
                {
                    Description = "Donations have been sync up to this datetime from PushPay",
                    ApplicationCode = "CRDS-FINANCE",
                    KeyName = "LastDonationSync",
                    Value = newSyncTime.ToUniversalTime().ToString("u")
                };
                await _configurationSettingsRepository.CreateConfigurationSettings(newEntry);
            }
            else
            {
                await _configurationSettingsRepository.UpdateValue(configId.Value,
                    newSyncTime.ToUniversalTime().ToString("u"));
            }
        }

        public async Task UpdateRecurringScheduleSyncTime(DateTime newSyncTime)
        {
            var configId =
                await _configurationSettingsRepository.GetConfigurationId("CRDS-FINANCE", "LastRecurringSync");
            if (!configId.HasValue)
            {
                var newEntry = new MpConfigurationSettings
                {
                    Description = "Recurring have been sync up to this datetime from PushPay",
                    ApplicationCode = "CRDS-FINANCE",
                    KeyName = "LastRecurringSync",
                    Value = newSyncTime.ToUniversalTime().ToString("u")
                };
                await _configurationSettingsRepository.CreateConfigurationSettings(newEntry);
            }
            else
            {
                await _configurationSettingsRepository.UpdateValue(configId.Value,
                    newSyncTime.ToUniversalTime().ToString("u"));
            }
        }
    }
}