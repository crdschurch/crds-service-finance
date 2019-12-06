using System;
using System.Collections.Generic;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Utilities.Logging;
using Crossroads.Service.Finance.Services.JournalEntry;
using System.Linq;

namespace Crossroads.Service.Finance.Services
{
    public class AdjustmentsToJournalEntriesService : IAdjustmentsToJournalEntriesService
    {
        private readonly IJournalEntryService _journalEntryService;

        public AdjustmentsToJournalEntriesService(IJournalEntryService journalEntryService)
        {
            _journalEntryService = journalEntryService;
        }

        public List<MpJournalEntry> Convert(List<MpDistributionAdjustment> mpDistributionAdjustments)
        {
            var journalEntries = new List<MpJournalEntry>();

            foreach (var mpDistributionAdjustment in mpDistributionAdjustments)
            {
                var matchingMpJournalEntry = journalEntries.FirstOrDefault(r => r.GL_Account_Number == mpDistributionAdjustment.GLAccountNumber &&
                                            r.AdjustmentYear == mpDistributionAdjustment.DonationDate.Year &&
                                            r.AdjustmentMonth == mpDistributionAdjustment.DonationDate.Month);

                if (matchingMpJournalEntry == null)
                {
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
