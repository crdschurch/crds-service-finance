using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services.JournalEntry;
using Crossroads.Service.Finance.Services.JournalEntryBatch;
using Exports.JournalEntries;
using Exports.Models;
using MinistryPlatform.Adjustments;
using MinistryPlatform.JournalEntries;
using MinistryPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Crossroads.Service.Finance.Services.Exports
{
    public class ExportService : IExportService
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IAdjustmentRepository _adjustmentRepository;
        private readonly IAdjustmentsToJournalEntriesService _adjustmentsToJournalEntriesService;
        private readonly IJournalEntryService _journalEntryService;
        private readonly IJournalEntryBatchService _batchService;
        private readonly IJournalEntryRepository _journalEntryRepository;
        private readonly IJournalEntryExport _journalEntryExport;
        private readonly IMapper _mapper;

        public ExportService(IAdjustmentRepository adjustmentRepository,
                             IAdjustmentsToJournalEntriesService adjustmentsToJournalEntriesService,
                             IJournalEntryService journalEntryService,
                             IJournalEntryBatchService batchService,
                             IJournalEntryRepository journalEntryRepository,
                             IJournalEntryExport journalEntryExport,
                             IMapper mapper)
        {
            _adjustmentRepository = adjustmentRepository;
            _adjustmentsToJournalEntriesService = adjustmentsToJournalEntriesService;
            _batchService = batchService;
            _journalEntryService = journalEntryService;
            _journalEntryRepository = journalEntryRepository;
            _journalEntryExport = journalEntryExport;
            _mapper = mapper;
        }

        public async void CreateJournalEntriesAsync()
        {
            var mpDistributionAdjustments = await _adjustmentRepository.GetUnprocessedDistributionAdjustments();

            List<MpJournalEntry> journalEntries = await _adjustmentsToJournalEntriesService.Convert(mpDistributionAdjustments);
            journalEntries = await _journalEntryService.AddBatchIdsAndClean(journalEntries);

            // create journal entries
            List<MpJournalEntry> mpJournalEntries =  await SaveJournalEntriesToMp(journalEntries);

            var markDistributionAdjustmentsAsProcessedInMpTask =
                Task.Run((() => MarkDistributionAdjustmentsAsProcessedInMp(mpDistributionAdjustments, mpJournalEntries)));

            await markDistributionAdjustmentsAsProcessedInMpTask;

            // update adjustments
            _adjustmentRepository.UpdateAdjustments(mpDistributionAdjustments);
        }

        public async Task<string> HelloWorld()
        {
            var result = await _journalEntryExport.HelloWorld();
            return result;
        }

        /// <summary>
        /// This pulls Journal Entries and exports them grouped by Batch Number. There should only be one Batch per day, but this code is written to handle
        /// instances where an export for a previous day may not have occurred (hence grouping exports by Batch Number). Velosio also needs each export to be grouped
        /// by Batch Number as well.
        ///
        /// Date of export is set to the current date, as this doesn't have anything to do with the original export date. Velosio needs this to be set to the same date
        /// the export is occurring to be able to validate the export.
        /// </summary>
        public async Task<string> ExportJournalEntries()
        {
            List<VelosioJournalEntryBatch> velosioJournalEntryStages = await CreateBatchesFromJournalEntries(true);

            // set totals here for validation
            decimal totalDebits = new decimal(0.0);
            decimal totalCredits = new decimal(0.0);

            var batchDataSetElement = new XElement("BatchDataSet", null);

            // convert entries to XML here
            foreach (var velosioJournalEntryBatch in velosioJournalEntryStages)
            {
                totalDebits += velosioJournalEntryBatch.TotalDebits;
                totalCredits += velosioJournalEntryBatch.TotalCredits;
                batchDataSetElement.Add(velosioJournalEntryBatch.BatchData);
            }

            int transactionCount = batchDataSetElement.Descendants("BatchDataTable").Count();
            var batchNumber = velosioJournalEntryStages.First().BatchNumber;

            return (await _journalEntryExport.ExportJournalEntryStage(
                batchNumber,
                totalDebits,
                totalCredits,
                transactionCount,
                batchDataSetElement)).ToString();
        }

        public async Task<string> ExportJournalEntriesManually(bool markExported = true)
        {
            var velosioJournalEntryStages = await CreateBatchesFromJournalEntries(markExported);

            var serializeJournalEntryStagesTask =
                Task.Run(() => SerializeJournalEntryStages(velosioJournalEntryStages));

            var serializedData = await serializeJournalEntryStagesTask;
            return serializedData;
        }

        public async Task<List<VelosioJournalEntryBatch>> CreateBatchesFromJournalEntries(bool markJournalEntriesAsProcessed = true)
        {
            List<MpJournalEntry> journalEntries = await _journalEntryRepository.GetUnexportedJournalEntries();

            var createJournalEntriesTask =
                Task.Run(() => _batchService.CreateBatchPerUniqueJournalEntryBatchId(journalEntries));

            List<VelosioJournalEntryBatch> batches = await createJournalEntriesTask;

            foreach (var journalEntry in journalEntries)
            {
                _batchService.AddJournalEntryToAppropriateBatch(batches, journalEntry, journalEntries.First().BatchID);

                if (markJournalEntriesAsProcessed == true)
                {
                    journalEntry.ExportedDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                }
            }

            if (markJournalEntriesAsProcessed)
            {
                await _journalEntryRepository.UpdateJournalEntries(journalEntries);
            }

            // Batch numbers need to be the same, or the total won't work
            batches.ForEach(b => b.BatchNumber = journalEntries.First().BatchID);

            return batches;
        }

        // we return only the journal entry info, not the metadata required by Velosio for this export
        public string SerializeJournalEntryStages(List<VelosioJournalEntryBatch> velosioJournalEntryStages)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Batch;Account;Debit;Credit;");

            foreach (var velosioJournalEntryStage in velosioJournalEntryStages)
            {
                // convert xml to new line in csv
                foreach (var batchDataElement in velosioJournalEntryStage.BatchData.Descendants("BatchDataTable"))
                {
                    var elementString = batchDataElement.Descendants("BatchNumber").First().Value + "," +
                                        batchDataElement.Descendants("Account").First().Value + "," +
                                        batchDataElement.Descendants("DebitAmount").First().Value + "," +
                                        batchDataElement.Descendants("CreditAmount").First().Value + ",";

                    stringBuilder.AppendLine(elementString);
                }
            }

            return stringBuilder.ToString();
        }

        private async Task<List<MpJournalEntry>> SaveJournalEntriesToMp(List<MpJournalEntry> journalEntries)
        {
            var mpJournalEntries = new List<MpJournalEntry>();
            if (journalEntries.Any())
            {
                mpJournalEntries = await _journalEntryRepository.CreateMpJournalEntries(journalEntries);
            }

            return mpJournalEntries;
        }

        private static void MarkDistributionAdjustmentsAsProcessedInMp(List<MpDistributionAdjustment> mpDistributionAdjustments,
                                                               List<MpJournalEntry> mpJournalEntries)
        {
            if (mpDistributionAdjustments != null && mpDistributionAdjustments.Any())
            {
                foreach (var mpDistributionAdjustment in mpDistributionAdjustments)
                {
                    mpDistributionAdjustment.ProcessedDate = DateTime.Now;

                    if (mpJournalEntries.Any())
                    {
                        mpDistributionAdjustment.JournalEntryId =
                            mpJournalEntries
                                .FirstOrDefault(r => r.GL_Account_Number == mpDistributionAdjustment.GLAccountNumber)?
                                .JournalEntryID;
                    }
                }
            }
        }
    }
}