using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Models;

namespace MinistryPlatform.JournalEntries
{
    public interface IJournalEntryRepository
    {
        Task<List<MpJournalEntry>> CreateMpJournalEntries(List<MpJournalEntry> mpJournalEntries);
        Task <List<MpJournalEntry>> GetUnexportedJournalEntries();
        Task UpdateJournalEntries(List<MpJournalEntry> mpJournalEntries);
    }
}
