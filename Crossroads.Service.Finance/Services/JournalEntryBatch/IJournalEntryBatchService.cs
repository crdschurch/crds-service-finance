using Exports.Models;
using MinistryPlatform.Models;
using System.Collections.Generic;

namespace Crossroads.Service.Finance.Services.JournalEntryBatch
{
    public interface IJournalEntryBatchService
    {
        List<VelosioJournalEntryBatch> CreateBatchPerUniqueJournalEntryBatchId(List<MpJournalEntry> journalEntries);
        void AddJournalEntryToAppropriateBatch(List<VelosioJournalEntryBatch> batches, MpJournalEntry journalEntry);
    }
}
