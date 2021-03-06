﻿using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services.JournalEntry;
using MinistryPlatform.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services
{
    public class AdjustmentsToJournalEntriesService : IAdjustmentsToJournalEntriesService
    {
        private readonly IJournalEntryService _journalEntryService;

        public AdjustmentsToJournalEntriesService(IJournalEntryService journalEntryService)
        {
            _journalEntryService = journalEntryService;
        }

        public async Task<List<MpJournalEntry>> Convert(List<MpDistributionAdjustment> mpDistributionAdjustments)
        {
            var journalEntries = new List<MpJournalEntry>();

            foreach (var mpDistributionAdjustment in mpDistributionAdjustments)
            {
                var matchingMpJournalEntry = journalEntries.FirstOrDefault(r => r.GL_Account_Number == mpDistributionAdjustment.GLAccountNumber &&
                                            r.AdjustmentYear == mpDistributionAdjustment.DonationDate.Year &&
                                            r.AdjustmentMonth == mpDistributionAdjustment.DonationDate.Month);

                if (matchingMpJournalEntry == null)
                {
                    var journalEntryTask = Task.Run(() =>
                        _journalEntryService.CreateNewJournalEntry(null, mpDistributionAdjustment));
                    journalEntries.Add(await journalEntryTask);

                    MpJournalEntry newJournalEntry = _journalEntryService.CreateNewJournalEntry(null, mpDistributionAdjustment);
                    journalEntries.Add(newJournalEntry);
                }
                else
                {
                    _journalEntryService.AdjustExistingJournalEntry(mpDistributionAdjustment, matchingMpJournalEntry);
                }
            }

            return journalEntries;
        }
    }
}
