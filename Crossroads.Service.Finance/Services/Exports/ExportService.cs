using AutoMapper;
using MinistryPlatform.Adjustments;
using System;
using System.Collections.Generic;
using System.Linq;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.JournalEntries;
using MinistryPlatform.Models;

namespace Crossroads.Service.Finance.Services.Exports
{
    public class ExportService: IExportService
    {
        private readonly IAdjustmentRepository _adjustmentRepository;
        private readonly IJournalEntryRepository _journalEntryRepository;
        private readonly IMapper _mapper;

        public ExportService(IAdjustmentRepository adjustmentRepository, IJournalEntryRepository journalEntryRepository, IMapper mapper)
        {
            _adjustmentRepository = adjustmentRepository;
            _journalEntryRepository = journalEntryRepository;
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
                        Amount = mpDistributionAdjustment.Amount,
                        BatchID = batchId,
                        CreatedDate = DateTime.Now,
                        ExportedDate = null,
                        Description = "test desc", // TODO: understand what goes here
                        GL_Account_Number = mpDistributionAdjustment.GLAccountNumber,
                        AdjustmentYear = mpDistributionAdjustment.DonationDate.Year,
                        AdjustmentMonth = mpDistributionAdjustment.DonationDate.Month
                    };

                    journalEntries.Add(mpJournalEntry);
                }
                else
                {
                    matchingMpJournalEntry.Amount = mpDistributionAdjustment.Amount;
                }

                // mark adjustment
                mpDistributionAdjustment.ProcessedDate = DateTime.Now;
            }

            // update adjustments
            _adjustmentRepository.UpdateAdjustments(mpDistributionAdjustments);

            // create journal entries
            _journalEntryRepository.CreateMpJournalEntries(journalEntries);
        }
    }
}
