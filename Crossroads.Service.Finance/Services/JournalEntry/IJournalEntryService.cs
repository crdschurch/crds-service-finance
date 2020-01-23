using MinistryPlatform.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services.JournalEntry
{
    public interface IJournalEntryService
    {
        Task<List<MpJournalEntry>> AddBatchIdsToJournalEntries(List<MpJournalEntry> entries);
        void AdjustExistingJournalEntry(MpDistributionAdjustment mpDistributionAdjustment, MpJournalEntry journalEntry);
        Task<List<MpJournalEntry>> AddBatchIdsAndClean(List<MpJournalEntry> journalEntries);
        Task<MpJournalEntry> NetCreditsAndDebits(MpJournalEntry journalEntry);
        MpJournalEntry CreateNewJournalEntry(string batchId, MpDistributionAdjustment mpDistributionAdjustment);
        List<MpJournalEntry> RemoveWashEntries(List<MpJournalEntry> journalEntries);
    }
}
