using Exports.Models;
using MinistryPlatform.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services.JournalEntryBatch
{
    public interface IJournalEntryBatchService
    {
        Task<List<VelosioJournalEntryBatch>> CreateBatchPerUniqueJournalEntryBatchId(List<MpJournalEntry> journalEntries);
        void AddJournalEntryToAppropriateBatch(List<VelosioJournalEntryBatch> batches, MpJournalEntry journalEntry, string sharedBatchIdForDisplay);
    }
}
