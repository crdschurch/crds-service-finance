using MinistryPlatform.Models;
using System.Collections.Generic;

namespace Crossroads.Service.Finance.Services.JournalEntry
{
    public interface IJournalEntryService
    {
        MpJournalEntry AdjustExistingJournalEntry(MpDistributionAdjustment mpDistributionAdjustment, MpJournalEntry journalEntry);
        MpJournalEntry NetCreditsAndDebits(MpJournalEntry journalEntry);
        MpJournalEntry CreateNewJournalEntry(string batchId, MpDistributionAdjustment mpDistributionAdjustment);
        List<MpJournalEntry> RemoveWashEntries(List<MpJournalEntry> journalEntries);
    }
}
