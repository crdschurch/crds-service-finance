﻿using AutoMapper;
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

namespace Crossroads.Service.Finance.Services.Exports
{
    public class ExportService: IExportService
    {
        private readonly IAdjustmentRepository _adjustmentRepository;
        private readonly IJournalEntryService _journalEntryService;
        private readonly IJournalEntryBatchService _batchService;
        private readonly IJournalEntryRepository _journalEntryRepository;
        private readonly IJournalEntryExport _journalEntryExport;
        private readonly IMapper _mapper;

        public ExportService(IAdjustmentRepository adjustmentRepository,
                             IJournalEntryService journalEntryService,
                             IJournalEntryBatchService batchService,
                             IJournalEntryRepository journalEntryRepository, 
                             IJournalEntryExport journalEntryExport,
                             IMapper mapper)
        {
            _adjustmentRepository = adjustmentRepository;
            _batchService = batchService;
            _journalEntryService = journalEntryService;
            _journalEntryRepository = journalEntryRepository;
            _journalEntryExport = journalEntryExport;
            _mapper = mapper;
        }

        public void CreateJournalEntries()
        {
            var mpDistributionAdjustments = _adjustmentRepository.GetUnprocessedDistributionAdjustments();

            var today = DateTime.Now;

            // TODO: We need to verify this batch ID with Finance and Velosio
            var batchId = $"CRJE{today.Year}{today.Month}{today.Day}01";

            List<MpJournalEntry> journalEntries = GroupAdjustmentsIntoJournalEntries(mpDistributionAdjustments, batchId);
            journalEntries.ForEach(e => _journalEntryService.NetCreditsAndDebits(e));
            journalEntries = _journalEntryService.RemoveWashEntries(journalEntries);

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
            var velosioJournalEntryStages = CreateJournalEntryStages(true);

            // TODO: uncomment when preliminary testing is complete
            //_journalEntryExport.ExportJournalEntryStage(velosioJournalEntryStage);
        }

        public string ExportJournalEntriesManually(bool markExported = true)
        {
            var velosioJournalEntryStages = CreateJournalEntryStages(markExported);
            var serializedData = SerializeJournalEntryStages(velosioJournalEntryStages);
            return serializedData;
        }

        public List<VelosioJournalEntryBatch> CreateJournalEntryStages(bool doMarkJournalEntriesAsProcessed = true)
        {
            List<MpJournalEntry> journalEntries = _journalEntryRepository.GetUnexportedJournalEntries();
            List<VelosioJournalEntryBatch> batches = _batchService.CreateBatchPerUniqueJournalEntryBatchId(journalEntries);

            foreach (var journalEntry in journalEntries)
            {
                _batchService.AddJournalEntryToAppropriateBatch(batches, journalEntry);

                if (doMarkJournalEntriesAsProcessed == true)
                {
                    journalEntry.ExportedDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                }
            }

            _journalEntryRepository.UpdateJournalEntries(journalEntries);
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

        private List<MpJournalEntry> GroupAdjustmentsIntoJournalEntries(List<MpDistributionAdjustment> mpDistributionAdjustments, string batchId)
        {
            var journalEntries = new List<MpJournalEntry>();

            foreach (var mpDistributionAdjustment in mpDistributionAdjustments)
            {
                var matchingMpJournalEntry = journalEntries.FirstOrDefault(r => r.GL_Account_Number == mpDistributionAdjustment.GLAccountNumber &&
                                            r.AdjustmentYear == mpDistributionAdjustment.DonationDate.Year &&
                                            r.AdjustmentMonth == mpDistributionAdjustment.DonationDate.Month);

                if (matchingMpJournalEntry == null)
                {
                    MpJournalEntry newJournalEntry = _journalEntryService.CreateNewJournalEntry(batchId, mpDistributionAdjustment);
                    journalEntries.Add(newJournalEntry);
                }
                else
                {
                    _journalEntryService.AdjustExistingJournalEntry(mpDistributionAdjustment, matchingMpJournalEntry);
                }
            }

            return journalEntries;
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