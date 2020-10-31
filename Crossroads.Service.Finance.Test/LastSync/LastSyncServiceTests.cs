using System;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Services;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform;
using MinistryPlatform.Models;
using Moq;
using Xunit;

namespace Crossroads.Service.Finance.Test.LastSync
{
    public class LastSyncServiceTests
    {
        private readonly Mock<IConfigurationSettingsRepository> _configurationSettingsRepository;
        private readonly Mock<IConfigurationWrapper> _configurationWrapper;

        private readonly LastSyncService _fixture;

        public LastSyncServiceTests()
        {
            _configurationSettingsRepository = new Mock<IConfigurationSettingsRepository>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);

            _fixture = new LastSyncService(_configurationSettingsRepository.Object, _configurationWrapper.Object);
        }

        [Fact]
        public async Task ShouldGetLastDonationSyncTime()
        {
            // Arrange
            var lastDonationSync = DateTime.Parse(DateTime.Now.ToUniversalTime().ToString());

            _configurationWrapper.Setup(m => m.GetMpConfigValueAsync("CRDS-FINANCE", "LastDonationSync", false))
                .ReturnsAsync(lastDonationSync.ToString());

            // Act
            var results = await _fixture.GetLastDonationSyncTime();

            // Assert
            Assert.Equal(lastDonationSync, results);
        }

        [Fact]
        public async Task ShouldMakeLastSyncAnHourBeforeWhenNotExisting()
        {
            // Arrange
            var newLastDonationSync = DateTime.Parse(DateTime.Now.ToUniversalTime().AddHours(-1).ToString());

            _configurationWrapper.Setup(m => m.GetMpConfigValueAsync("CRDS-FINANCE", "LastDonationSync", false))
                .ReturnsAsync((string) null);
            _configurationSettingsRepository
                .Setup(m => m.CreateConfigurationSettings(It.IsAny<MpConfigurationSettings>()))
                .Returns(Task.CompletedTask);


            // Act
            var results = await _fixture.GetLastDonationSyncTime();

            // Assert
            Assert.Equal(newLastDonationSync, results);
            _configurationSettingsRepository.Verify(
                m => m.CreateConfigurationSettings(It.Is<MpConfigurationSettings>(cs =>
                    cs.Value == newLastDonationSync.ToString() && cs.KeyName == "LastDonationSync")), Times.Once);
        }

        [Fact]
        public async Task ShouldGetLastRecurringSyncTime()
        {
            // Arrange
            var lastRecurringSync = DateTime.Parse(DateTime.Now.ToUniversalTime().ToString());

            _configurationWrapper.Setup(m => m.GetMpConfigValueAsync("CRDS-FINANCE", "LastRecurringSync", false))
                .ReturnsAsync(lastRecurringSync.ToString());

            // Act
            var results = await _fixture.GetLastRecurringScheduleSyncTime();

            // Assert
            Assert.Equal(lastRecurringSync, results);
        }

        [Fact]
        public async Task ShouldMakeLastRecurringSyncWhenNotExisting()
        {
            // Arrange
            var newLastRecurringSync = DateTime.Parse(DateTime.Now.ToUniversalTime().AddHours(-1).ToString());

            _configurationWrapper.Setup(m => m.GetMpConfigValueAsync("CRDS-FINANCE", "LastRecurringSync", false))
                .ReturnsAsync((string) null);
            _configurationSettingsRepository
                .Setup(m => m.CreateConfigurationSettings(It.IsAny<MpConfigurationSettings>()))
                .Returns(Task.CompletedTask);


            // Act
            var results = await _fixture.GetLastRecurringScheduleSyncTime();

            // Assert
            Assert.Equal(newLastRecurringSync, results);
            _configurationSettingsRepository.Verify(
                m => m.CreateConfigurationSettings(It.Is<MpConfigurationSettings>(cs =>
                    cs.Value == newLastRecurringSync.ToString() && cs.KeyName == "LastRecurringSync")), Times.Once);
        }
    }
}