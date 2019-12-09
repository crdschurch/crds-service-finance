﻿using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace MinistryPlatform.JournalEntries
{
    public interface IJournalEntryRepository
    {
        List<MpJournalEntry> CreateMpJournalEntries(List<MpJournalEntry> mpJournalEntries);
        List<MpJournalEntry> GetUnexportedJournalEntries();
        void UpdateJournalEntries(List<MpJournalEntry> mpJournalEntries);
        List<string> GetCurrentDateBatchIds();
    }
}
