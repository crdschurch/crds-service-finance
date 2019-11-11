﻿using Moq;
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

namespace Crossroads.Service.Finance.Test.Exports
{
    public class ExportServiceTest
    {
        private readonly Mock<IAdjustmentRepository> _adjustmentRepository;
        private readonly Mock<IJournalEntryService> _journalEntryService;
        private readonly Mock<IJournalEntryBatchService> _batchService;
        private readonly Mock<IJournalEntryRepository> _journalEntryRepository;
        private readonly Mock<IJournalEntryExport> _journalEntryExport;
        private readonly Mock<IMapper> _mapper;

        private readonly IExportService _fixture;

        public ExportServiceTest()
        {
            _adjustmentRepository = new Mock<IAdjustmentRepository>();
            _journalEntryService = new Mock<IJournalEntryService>();
            _batchService = new Mock<IJournalEntryBatchService>();
            _journalEntryRepository = new Mock<IJournalEntryRepository>();
            _journalEntryExport = new Mock<IJournalEntryExport>();
            _mapper = new Mock<IMapper>();

            _fixture = new ExportService(_adjustmentRepository.Object,
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
                .Returns(new Task<List<MpDistributionAdjustment>>(() => MpDistributionAdjustmentMock.CreateList()));

            _journalEntryService.Setup(e => e.NetCreditsAndDebits(It.IsAny<MpJournalEntry>())).Returns(It.IsAny<MpJournalEntry>());

            _journalEntryService.Setup(e => e.RemoveWashEntries(It.IsAny<List<MpJournalEntry>>())).Returns(new List<MpJournalEntry>() { null });

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
            var mpJournalEntryMock = MpJournalEntryMock.CreateList();

            _journalEntryService.Setup(e => e.NetCreditsAndDebits(It.IsAny<MpJournalEntry>())).Returns(It.IsAny<MpJournalEntry>());

            _journalEntryService.Setup(e => e.RemoveWashEntries(It.IsAny<List<MpJournalEntry>>())).Returns(mpJournalEntryMock);

            _adjustmentRepository.Setup(r => r.GetUnprocessedDistributionAdjustments()).Returns(Task.FromResult(mpAdjustmentsMock));

            _journalEntryRepository.Setup(r => r.CreateMpJournalEntries(It.IsAny<List<MpJournalEntry>>())).Returns(Task.FromResult(mpJournalEntryMock));

            _adjustmentRepository.Setup(r => r.UpdateAdjustments(It.IsAny<List<MpDistributionAdjustment>>()));

            // Act
            _fixture.CreateJournalEntries();

            // Assert
            _journalEntryRepository.VerifyAll();
        }

        [Fact]
        public void ShouldSubmitJournalEntriesForExport()
        {
            // TODO: Reimplement this test once we're ready to start pushing data to Velosio
            //// Arrange
            //var mpJournalEntriesMock = MpJournalEntryMock.CreateList();

            //var batchId = "CRJE20190903";
            //var creditAmount = 5.0m;
            //var debitAmount = 5.0m;
            //var batchDate = new DateTime(2019, 09, 03);
            //var batchData = "batchData";
            //var transactionCount = 5;

            //var velosioJournalEntryStage = new VelosioJournalEntryStage
            //{
            //    BatchNumber = batchId,
            //    BatchDate = batchDate,
            //    TotalDebits = debitAmount,
            //    TotalCredits = creditAmount,
            //    BatchData = new XElement("batchdata", batchData),
            //    TransactionCount = transactionCount
            //};

            //_journalEntryRepository.Setup(m => m.GetMpJournalEntries()).Returns(mpJournalEntriesMock);
            //_journalEntryExport.Setup(m => m.ExportJournalEntryStage(It.IsAny<VelosioJournalEntryStage>()));

            //// Act
            //_fixture.ExportJournalEntries();

            //// Assert
            //_journalEntryExport.VerifyAll();
        }
    }
}
