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

            if (!string.IsNullOrWhiteSpace(lastSync)) return DateTime.Parse(lastSync);

            var newEntry = new MpConfigurationSettings
            {
                Description = "Donations have been sync up to this datetime from PushPay",
                ApplicationCode = "CRDS-FINANCE",
                KeyName = "LastDonationSync",
                Value = DateTime.Now.AddHours(-1).ToString()
            };
            await _configurationSettingsRepository.CreateConfigurationSettings(newEntry);
            return DateTime.Parse(newEntry.Value);
        }

        public async Task<DateTime> GetLastRecurringScheduleSyncTime()
        {
            var lastSync = await _configurationWrapper.GetMpConfigValueAsync("CRDS-FINANCE", "LastRecurringSync");

            if (!string.IsNullOrWhiteSpace(lastSync)) return DateTime.Parse(lastSync);

            var newEntry = new MpConfigurationSettings
            {
                Description = "Recurring have been sync up to this datetime from PushPay",
                ApplicationCode = "CRDS-FINANCE",
                KeyName = "LastRecurringSync",
                Value = DateTime.Now.AddHours(-1).ToString()
            };
            await _configurationSettingsRepository.CreateConfigurationSettings(newEntry);
            return DateTime.Parse(newEntry.Value);
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
                    Value = newSyncTime.ToUniversalTime().ToString()
                };
                await _configurationSettingsRepository.CreateConfigurationSettings(newEntry);
            }
            else
            {
                await _configurationSettingsRepository.UpdateValue(configId.Value,
                    newSyncTime.ToUniversalTime().ToString());
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
                    Value = newSyncTime.ToUniversalTime().ToString()
                };
                await _configurationSettingsRepository.CreateConfigurationSettings(newEntry);
            }
            else
            {
                await _configurationSettingsRepository.UpdateValue(configId.Value,
                    newSyncTime.ToUniversalTime().ToString());
            }
        }
    }
}