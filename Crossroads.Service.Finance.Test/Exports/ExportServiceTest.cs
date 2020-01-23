using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using Crossroads.Service.Finance.Services.Exports;
using Exports.JournalEntries;
using Exports.Models;
using MinistryPlatform.Adjustments;
using MinistryPlatform.JournalEntries;
using MinistryPlatform.Models;
using Mock;
using Xunit;
using Crossroads.Service.Finance.Services.JournalEntryBatch;
using Crossroads.Service.Finance.Services.JournalEntry;
using Crossroads.Service.Finance.Interfaces;

namespace Crossroads.Service.Finance.Test.Exports
{
    public class ExportServiceTest
    {
        private readonly Mock<IAdjustmentRepository> _adjustmentRepository;
        private readonly Mock<IAdjustmentsToJournalEntriesService> _adjustmentsToJournalEntriesService;
        private readonly Mock<IJournalEntryService> _journalEntryService;
        private readonly Mock<IJournalEntryBatchService> _batchService;
        private readonly Mock<IJournalEntryRepository> _journalEntryRepository;
        private readonly Mock<IJournalEntryExport> _journalEntryExport;
        private readonly Mock<IMapper> _mapper;

        private readonly IExportService _fixture;

        public ExportServiceTest()
        {
            _adjustmentRepository = new Mock<IAdjustmentRepository>();
            _adjustmentsToJournalEntriesService = new Mock<IAdjustmentsToJournalEntriesService>();
            _journalEntryService = new Mock<IJournalEntryService>();
            _batchService = new Mock<IJournalEntryBatchService>();
            _journalEntryRepository = new Mock<IJournalEntryRepository>();
            _journalEntryExport = new Mock<IJournalEntryExport>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>();

            _fixture = new ExportService(_adjustmentRepository.Object,
                                        _adjustmentsToJournalEntriesService.Object,
                                        _journalEntryService.Object,
                                        _batchService.Object,
                                        _journalEntryRepository.Object, 
                                        _journalEntryExport.Object,
                                        _mapper.Object);
        }

        [Fact]
        public void ShouldCreateNoJournalEntriesIfNoAdjustments()
        {
            // Arrange
            _adjustmentRepository.Setup(r => r.GetUnprocessedDistributionAdjustments())
                .Returns(Task.FromResult<List<MpDistributionAdjustment>>(null));

            _journalEntryService.Setup(e => e.AddBatchIdsAndClean(
                It.IsAny<List<MpJournalEntry>>())).Returns(Task.FromResult(new List<MpJournalEntry>() { null }));

            // Act
            _fixture.CreateJournalEntriesAsync();

            // Assert
            _journalEntryRepository.VerifyAll();
        }

        [Fact]
        public void ShouldCreateJournalEntriesIfAdjustmentsAvailable()
        {
            // Arrange
            var mpAdjustmentsMock = MpDistributionAdjustmentMock.CreateList();
            var mpJournalEntryMock = MpJournalEntryMock.CreateList();

            _journalEntryService.Setup(e => e.AddBatchIdsAndClean(It.IsAny<List<MpJournalEntry>>())).Returns(Task.FromResult(mpJournalEntryMock));

            _adjustmentRepository.Setup(r => r.GetUnprocessedDistributionAdjustments()).Returns(Task.FromResult(mpAdjustmentsMock));

            _journalEntryRepository.Setup(r => r.CreateMpJournalEntries(It.IsAny<List<MpJournalEntry>>())).Returns(Task.FromResult(mpJournalEntryMock));

            _adjustmentRepository.Setup(r => r.UpdateAdjustments(It.IsAny<List<MpDistributionAdjustment>>()));

            // Act
            _fixture.CreateJournalEntriesAsync();

            // Assert
            _journalEntryRepository.VerifyAll();
        }

        [Fact]
        public void ShouldSubmitJournalEntriesForExport()
        {
            // Arrange
            var mpJournalEntriesMock = MpJournalEntryMock.CreateList();

            var batchId = "CRJE20190903";
            var creditAmount = 5.0m;
            var debitAmount = 5.0m;
            var batchDate = new DateTime(2019, 09, 03);
            var batchData = "batchData";
            var transactionCount = 0;

            var velosioJournalEntryBatch = new VelosioJournalEntryBatch("CRJE20190903")
            {
                BatchNumber = batchId,
                BatchDate = batchDate,
                TotalDebits = debitAmount,
                TotalCredits = creditAmount,
                BatchData = new XElement("batchData", batchData),
                TransactionCount = transactionCount
            };
            var mockBatchList = new List<VelosioJournalEntryBatch>
            {
                velosioJournalEntryBatch
            };

            _journalEntryRepository.Setup(m => m.GetUnexportedJournalEntries()).Returns(Task.FromResult(mpJournalEntriesMock));
            _batchService.Setup(m => m.CreateBatchPerUniqueJournalEntryBatchId(It.IsAny<List<MpJournalEntry>>()))
                .Returns(Task.FromResult(mockBatchList));

            _journalEntryExport.Setup<Task>(m => m.ExportJournalEntryStage(
                It.IsAny<string>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                It.IsAny<int>(),
                It.IsAny<System.Xml.Linq.XElement>()
            )).Returns(Task.FromResult<string>("test"));

            // Act
            var result = _fixture.ExportJournalEntries().Result;

            // Assert
            _journalEntryExport.VerifyAll();
        }
    }
}
