using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Service.Finance.Services.Exports;
using MinistryPlatform.Adjustments;
using MinistryPlatform.JournalEntries;
using MinistryPlatform.Models;
using Mock;
using Xunit;

namespace Crossroads.Service.Finance.Test.Exports
{
    public class ExportServiceTest
    {
        private readonly Mock<IAdjustmentRepository> _adjustmentRepository;
        private readonly Mock<IJournalEntryRepository> _journalEntryRepository;
        private readonly Mock<IMapper> _mapper;

        private readonly IExportService _fixture;

        public ExportServiceTest()
        {
            _adjustmentRepository = new Mock<IAdjustmentRepository>();
            _journalEntryRepository = new Mock<IJournalEntryRepository>();
            _mapper = new Mock<IMapper>();

            _fixture = new ExportService(_adjustmentRepository.Object, _journalEntryRepository.Object, _mapper.Object);
        }

        [Fact]
        public void ShouldCreateNoJournalEntriesIfNoAdjustments()
        {
            // Arrange
            _adjustmentRepository.Setup(r => r.GetAdjustmentsByDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new List<MpDistributionAdjustment>());

            _journalEntryRepository.Setup(r => r.CreateOrUpdateMpJournalEntries(It.IsAny<List<MpJournalEntry>>()));

            // Act
            _fixture.CreateJournalEntries();

            // Assert
            _journalEntryRepository.VerifyAll();
        }

        [Fact]
        public void ShouldCreateJournalEntriesIfAdjustmentsAvailable()
        {
            // Arrange
            var mpAdjustmentsMock = MpDistributionAdjustmentMock.CreateList();

            _adjustmentRepository.Setup(r => r.GetAdjustmentsByDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(mpAdjustmentsMock);

            _adjustmentRepository.Setup(r => r.UpdateAdjustments(It.IsAny<List<MpDistributionAdjustment>>()));

            _journalEntryRepository.Setup(r => r.CreateOrUpdateMpJournalEntries(It.IsAny<List<MpJournalEntry>>()));

            // Act
            _fixture.CreateJournalEntries();

            // Assert
            _journalEntryRepository.VerifyAll();
        }
    }
}
