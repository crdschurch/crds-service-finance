using AutoMapper;
using MinistryPlatform.Adjustments;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Crossroads.Service.Finance.Models;
using Exports.JournalEntries;
using Exports.Models;
using MinistryPlatform.JournalEntries;
using MinistryPlatform.Models;

namespace Crossroads.Service.Finance.Services.Exports
{
    public class ExportService: IExportService
    {
        private readonly IAdjustmentRepository _adjustmentRepository;
        private readonly IJournalEntryRepository _journalEntryRepository;
        private readonly IJournalEntryExport _journalEntryExport;
        private readonly IMapper _mapper;

        public ExportService(IAdjustmentRepository adjustmentRepository, IJournalEntryRepository journalEntryRepository, IJournalEntryExport journalEntryExport, IMapper mapper)
        {
            _adjustmentRepository = adjustmentRepository;
            _journalEntryRepository = journalEntryRepository;
            _journalEntryExport = journalEntryExport;
            _mapper = mapper;
        }

        public void CreateJournalEntries()
        {
            // get adjustments that are not exported
            var yesterday = DateTime.Now;
            var startDate = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day);
            var endDate = yesterday.AddDays(1);
            var mpDistributionAdjustments = _adjustmentRepository.GetAdjustmentsByDate(startDate, endDate);

            // TODO: We need to verify this batch ID with Finance and Velosio
            var batchId = $"CRJE{yesterday.Year}{yesterday.Month}{yesterday.Day}01";

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
                        mpJournalEntry.DebitAmount = mpDistributionAdjustment.Amount;
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
                        matchingMpJournalEntry.DebitAmount = mpDistributionAdjustment.Amount;
                    }
                }

                // mark adjustment
                mpDistributionAdjustment.ProcessedDate = DateTime.Now;
            }

            // update adjustments
            _adjustmentRepository.UpdateAdjustments(mpDistributionAdjustments);

            // create journal entries
            _journalEntryRepository.CreateMpJournalEntries(journalEntries);
        }

        public void HelloWorld()
        {
            var result = _journalEntryExport.HelloWorld().Result;
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
            // pull all journal entries that have not been processed
            var journalEntries = _journalEntryRepository.GetMpJournalEntries();

            var velosioJournalEntryStages = new List<VelosioJournalEntryStage>();

            // total up journal entry metadata
            foreach (var journalEntry in journalEntries)
            {
                var velosioJournalEntryStage =
                    velosioJournalEntryStages.FirstOrDefault(r => r.BatchNumber == journalEntry.BatchID);

                if (velosioJournalEntryStage == null)
                {
                    velosioJournalEntryStage = new VelosioJournalEntryStage
                    {
                        BatchNumber = journalEntry.BatchID,
                        TotalCredits = journalEntry.DebitAmount,
                        TotalDebits = journalEntry.CreditAmount,
                        BatchDate = DateTime.Parse(DateTime.Now.ToShortDateString()),
                        BatchData = new XElement("BatchDataSet", null)
                    };

                    velosioJournalEntryStage.BatchData.Add(SerializeJournalEntry(journalEntry));
                    velosioJournalEntryStages.Add(velosioJournalEntryStage);
                }
                else
                {
                    velosioJournalEntryStage.BatchData.Add(SerializeJournalEntry(journalEntry));
                    velosioJournalEntryStage.TotalCredits += journalEntry.CreditAmount;
                    velosioJournalEntryStage.TotalDebits += journalEntry.DebitAmount;
                }

                velosioJournalEntryStage.TransactionCount++;

                journalEntry.ExportedDate = DateTime.Parse(DateTime.Now.ToShortDateString());

                _journalEntryExport.ExportJournalEntryStage(velosioJournalEntryStage);

                //_journalEntryExport.ExportJournalEntryStage(velosioJournalEntryStage.BatchNumber, velosioJournalEntryStage.TotalCredits, velosioJournalEntryStage.TotalDebits, DateTime.Today, velosioJournalEntryStage.BatchData.ToString(), velosioJournalEntryStage.)
            }

            _journalEntryRepository.UpdateJournalEntries(journalEntries);
        }

        private XElement SerializeJournalEntry(MpJournalEntry mpJournalEntry)
        {
            var journalEntryXml = new XElement("BatchDataTable", null);
            journalEntryXml.Add(new XElement("BatchNumber", mpJournalEntry.BatchID));
            journalEntryXml.Add(new XElement("Reference", mpJournalEntry.GetReferenceString()));
            journalEntryXml.Add(new XElement("TransactionDate", DateTime.Now.Date));
            journalEntryXml.Add(new XElement("Account", mpJournalEntry.GL_Account_Number));
            journalEntryXml.Add(new XElement("DebitAmount", mpJournalEntry.DebitAmount));
            journalEntryXml.Add(new XElement("CreditAmount", mpJournalEntry.CreditAmount));

            return journalEntryXml;
        }
    }
}
