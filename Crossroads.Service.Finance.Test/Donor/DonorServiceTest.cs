using System;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Services.Donor;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Donors;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Mock;
using Moq;
using Newtonsoft.Json;
using Pushpay.Models;
using Xunit;

namespace Crossroads.Service.Finance.Test.Donor
{
    public class DonorServiceTest
    {
        private readonly Mock<IDonorRepository> _donorRepository;
        private readonly Mock<IContactRepository> _contactRepository;
        private readonly Mock<IConfigurationWrapper> _configurationWrapper;

        private readonly IDonorService _fixture;

        public DonorServiceTest()
        {
            _donorRepository = new Mock<IDonorRepository>();
            _contactRepository = new Mock<IContactRepository>();
            _configurationWrapper = new Mock<IConfigurationWrapper>();

            _fixture = new DonorService(_donorRepository.Object, _configurationWrapper.Object,
                _contactRepository.Object);
        }

        [Fact]
        public async Task CreateDonorFromMp()
        {
            // Arrange 
            var newDonor = new MpDonor
            {
                ContactId = 700324,
                StatementFrequencyId = 1, // quarterly
                StatementTypeId = 1, // individual
                StatementMethodId = 2, // email+online
                SetupDate = DateTime.Now
            };

            var savedDonor = new MpDonor
            {
                DonorId = 909090,
                ContactId = 700324,
                StatementFrequencyId = 1, // quarterly
                StatementTypeId = 1, // individual
                StatementMethodId = 2, // email+online
                SetupDate = DateTime.Now
            };
            _donorRepository.Setup(m => m.CreateDonor(newDonor)).ReturnsAsync(savedDonor);

            // Act
            var results = await _fixture.CreateDonor(newDonor);

            // Assert
            Assert.Equal(savedDonor, results);
        }

        [Fact]
        public async Task CreateDonorFromPushpay()
        {
            // Arrange
            var scheduleToProcess =
                JsonConvert.DeserializeObject<PushpayRecurringGiftDto>(MockPushPayRecurringSchedules.GetListOfOne()[0]
                    .RawJson);
            var contactId = 78979;
            var savedDonor = new MpDonor
            {
                ContactId = contactId,
                DonorId = 779089,
            };

            _contactRepository.Setup(m => m.MatchContact(scheduleToProcess.Payer.FirstName,
                scheduleToProcess.Payer.LastName,
                scheduleToProcess.Payer.MobileNumber, scheduleToProcess.Payer.EmailAddress)).ReturnsAsync(new MpDonor
            {
                ContactId = contactId
            });
            _donorRepository.Setup(m => m.CreateDonor(It.IsAny<MpDonor>())).ReturnsAsync(savedDonor);

            // Act
            var result = await _fixture.CreateDonor(scheduleToProcess);

            // Assert 
            Assert.Equal(savedDonor.ContactId, result.ContactId);
            Assert.Equal(savedDonor.DonorId, result.DonorId);
        }

        [Fact]
        public async Task CreateDonorFromPushpayDonorFound()
        {
            // Arrange
            var scheduleToProcess =
                JsonConvert.DeserializeObject<PushpayRecurringGiftDto>(MockPushPayRecurringSchedules.GetListOfOne()[0]
                    .RawJson);
            var contactId = 78979;
            var savedDonor = new MpDonor
            {
                ContactId = contactId,
                DonorId = 779089,
            };

            _contactRepository.Setup(m => m.MatchContact(scheduleToProcess.Payer.FirstName,
                scheduleToProcess.Payer.LastName,
                scheduleToProcess.Payer.MobileNumber, scheduleToProcess.Payer.EmailAddress)).ReturnsAsync(savedDonor);

            // Act
            var result = await _fixture.CreateDonor(scheduleToProcess);

            // Assert 
            Assert.Equal(savedDonor, result);
        }

        [Fact]
        public async Task CreateDonorFromPushpayDefaultDonor()
        {
            // Arrange
            var scheduleToProcess =
                JsonConvert.DeserializeObject<PushpayRecurringGiftDto>(MockPushPayRecurringSchedules.GetListOfOne()[0]
                    .RawJson);
            MpDonor matchContact = null;
            var savedDonor = new MpDonor
            {
                DonorId = 1,
                CongregationId = 5
            };

            _contactRepository.Setup(m => m.MatchContact(scheduleToProcess.Payer.FirstName,
                scheduleToProcess.Payer.LastName,
                scheduleToProcess.Payer.MobileNumber, scheduleToProcess.Payer.EmailAddress)).ReturnsAsync(matchContact);

            // Act
            var result = await _fixture.CreateDonor(scheduleToProcess);

            // Assert 
            Assert.Equal(savedDonor.CongregationId, result.CongregationId);
            Assert.Equal(savedDonor.DonorId, result.DonorId);
        }

        [Fact]
        public async Task TestFindDonorIdFound()
        {
            // Arrange
            var scheduleToProcess =
                JsonConvert.DeserializeObject<PushpayRecurringGiftDto>(MockPushPayRecurringSchedules.GetListOfOne()[0]
                    .RawJson);
            _donorRepository.Setup(m => m.GetDonorIdByProcessorId(scheduleToProcess.Payer.Key)).ReturnsAsync(123);
            
            // Act
            var result = await _fixture.FindDonorId(scheduleToProcess);
            
            // Assert
            Assert.True(result.HasValue);
            Assert.Equal(123, result.Value);
        }

        [Fact]
        public async Task TestFindDonorIdNotFound()
        {
            // Arrange
            int? id = null;
            var scheduleToProcess =
                JsonConvert.DeserializeObject<PushpayRecurringGiftDto>(MockPushPayRecurringSchedules.GetListOfOne()[0]
                    .RawJson);
            _donorRepository.Setup(m => m.GetDonorIdByProcessorId(scheduleToProcess.Payer.Key)).ReturnsAsync(id);
            
            // Act
            var result = await _fixture.FindDonorId(scheduleToProcess);
            
            // Assert
            Assert.False(result.HasValue);
        }
    }
}