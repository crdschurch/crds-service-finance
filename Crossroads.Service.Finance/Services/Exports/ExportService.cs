using AutoMapper;
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
        private readonly IJournalEntryBatchService _batchService;
        private readonly IJournalEntryRepository _journalEntryRepository;
        private readonly IJournalEntryExport _journalEntryExport;
        private readonly IMapper _mapper;

        public ExportService(IAdjustmentRepository adjustmentRepository,
                             IJournalEntryBatchService batchService,
                             IJournalEntryRepository journalEntryRepository, 
                             IJournalEntryExport journalEntryExport,
                             IMapper mapper)
        {
            _adjustmentRepository = adjustmentRepository;
            _batchService = batchService;
            _journalEntryRepository = journalEntryRepository;
            _journalEntryExport = journalEntryExport;
            _mapper = mapper;
        }

        public void CreateJournalEntries()
        {
            // get adjustments that haven't been processed yet
            var mpDistributionAdjustments = _adjustmentRepository.GetUnprocessedDistributionAdjustments();

            var today = DateTime.Now;

            // TODO: We need to verify this batch ID with Finance and Velosio
            var batchId = $"CRJE{today.Year}{today.Month}{today.Day}01";

            var journalEntries = new List<MpJournalEntry>();

            foreach (var mpDistributionAdjustment in mpDistributionAdjustments)
            {
                var matchingMpJournalEntry = journalEntries.FirstOrDefault(r => r.GL_Account_Number == mpDistributionAdjustment.GLAccountNumber &&
                                            r.AdjustmentYear == mpDistributionAdjustment.DonationDate.Year &&
                                            r.AdjustmentMonth == mpDistributionAdjustment.DonationDate.Month);

                // TODO: verify if this will ever be null
                if (matchingMpJournalEntry == null)
                {
                    var mpJournalEntry = new MpJournalEntry
                    {
                        BatchID = batchId,
                        CreatedDate = DateTime.Now,
                        ExportedDate = null,
                        Description = "test desc", // TODO: understand what goes here
                        GL_Account_Number = mpDistributionAdjustment.GLAccountNumber,
                        AdjustmentYear = mpDistributionAdjustment.DonationDate.Year,
                        AdjustmentMonth = mpDistributionAdjustment.DonationDate.Month
                    };

                    if (Math.Sign(mpDistributionAdjustment.Amount) == 1)
                    {
                        mpJournalEntry.CreditAmount = mpDistributionAdjustment.Amount;
                    }
                    else
                    {
                        mpJournalEntry.DebitAmount = Math.Abs(mpDistributionAdjustment.Amount);
                    }

                    journalEntries.Add(mpJournalEntry);
                }
                else
                {
                    if (Math.Sign(mpDistributionAdjustment.Amount) == 1)
                    {
                        matchingMpJournalEntry.CreditAmount = mpDistributionAdjustment.Amount;
                    }
                    else
                    {
                        matchingMpJournalEntry.DebitAmount = Math.Abs(mpDistributionAdjustment.Amount);
                    }
                }
            }

            // create journal entries
            var mpJournalEntries = _journalEntryRepository.CreateMpJournalEntries(journalEntries);

            foreach (var mpDistributionAdjustment in mpDistributionAdjustments)
            {
                mpDistributionAdjustment.ProcessedDate = DateTime.Now;

                // TODO: verify that this will not cause issues with the wrong adjustment being keyed to the wrong journal entry
                mpDistributionAdjustment.JournalEntryId = mpJournalEntries
                    .First(r => r.GL_Account_Number == mpDistributionAdjustment.GLAccountNumber).JournalEntryID;
            }

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
    }
}
