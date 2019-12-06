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
using System.Xml.Linq;

namespace Crossroads.Service.Finance.Services.Exports
{
    public class ExportService: IExportService
    {
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

        public void CreateJournalEntries()
        {
            var mpDistributionAdjustments = _adjustmentRepository.GetUnprocessedDistributionAdjustments();

            List<MpJournalEntry> journalEntries = _adjustmentsToJournalEntriesService.Convert(mpDistributionAdjustments);
            journalEntries = _journalEntryService.AddBatchIdsAndClean(journalEntries);

            // create journal entries
            List<MpJournalEntry> mpJournalEntries = SaveJournalEntriesToMp(journalEntries);

            MarkDistributionAdjustmentsAsProcessedInMp(mpDistributionAdjustments, mpJournalEntries);

            // update adjustments
            _adjustmentRepository.UpdateAdjustments(mpDistributionAdjustments);
        }

        public string HelloWorld()
        {
            var result = _journalEntryExport.HelloWorld().Result;
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
        public void ExportJournalEntries()
        {
            List<VelosioJournalEntryBatch> velosioJournalEntryStages = CreateBatchesFromJournalEntries(true);

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

            _journalEntryExport.ExportJournalEntryStage(
                velosioJournalEntryStages.First().BatchNumber,
                totalDebits,
                totalCredits,
                transactionCount,
                batchDataSetElement);
        }

        public string ExportJournalEntriesManually(bool markExported = true)
        {
            var velosioJournalEntryStages = CreateBatchesFromJournalEntries(markExported);
            var serializedData = SerializeJournalEntryStages(velosioJournalEntryStages);
            return serializedData;
        }

        public List<VelosioJournalEntryBatch> CreateBatchesFromJournalEntries(bool markJournalEntriesAsProcessed = true)
        {
            List<MpJournalEntry> journalEntries = _journalEntryRepository.GetUnexportedJournalEntries();
            List<VelosioJournalEntryBatch> batches = _batchService.CreateBatchPerUniqueJournalEntryBatchId(journalEntries);

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
                _journalEntryRepository.UpdateJournalEntries(journalEntries);
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

        private List<MpJournalEntry> SaveJournalEntriesToMp(List<MpJournalEntry> journalEntries)
        {
            var mpJournalEntries = new List<MpJournalEntry>();
            if (journalEntries.Any())
            {
                mpJournalEntries = _journalEntryRepository.CreateMpJournalEntries(journalEntries);
            }

            return mpJournalEntries;
        }

        private static void MarkDistributionAdjustmentsAsProcessedInMp(List<MpDistributionAdjustment> mpDistributionAdjustments,
                                                               List<MpJournalEntry> mpJournalEntries)
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
