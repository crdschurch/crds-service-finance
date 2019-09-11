using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace MinistryPlatform.JournalEntries
{
    public interface IJournalEntryRepository
    {
        List<MpJournalEntry> CreateMpJournalEntries(List<MpJournalEntry> mpJournalEntries);
        List<MpJournalEntry> GetMpJournalEntriesWhichHaventBeenExported();
        void UpdateJournalEntries(List<MpJournalEntry> mpJournalEntries);
    }
}
