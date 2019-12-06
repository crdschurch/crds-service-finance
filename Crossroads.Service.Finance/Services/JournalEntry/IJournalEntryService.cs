using MinistryPlatform.Models;
using System.Collections.Generic;

namespace Crossroads.Service.Finance.Services.JournalEntry
{
    public interface IJournalEntryService
    {
        List<MpJournalEntry> AddBatchIdsToJournalEntries(List<MpJournalEntry> entries);
        void AdjustExistingJournalEntry(MpDistributionAdjustment mpDistributionAdjustment, MpJournalEntry journalEntry);
        List<MpJournalEntry> AddBatchIdsAndClean(List<MpJournalEntry> journalEntries);
        MpJournalEntry NetCreditsAndDebits(MpJournalEntry journalEntry);
        MpJournalEntry CreateNewJournalEntry(string batchId, MpDistributionAdjustment mpDistributionAdjustment);
        List<MpJournalEntry> RemoveWashEntries(List<MpJournalEntry> journalEntries);
    }
}
