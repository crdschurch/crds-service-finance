using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace MinistryPlatform.JournalEntries
{
    public interface IJournalEntryRepository
    {
        List<MpJournalEntry> CreateOrUpdateMpJournalEntries(List<MpJournalEntry> mpJournalEntries);
    }
}
