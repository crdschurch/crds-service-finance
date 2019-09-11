using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Exports.Models;

namespace Exports.JournalEntries
{
    public interface IJournalEntryExport
    {
        Task<string> HelloWorld();
        Task<string> ExportJournalEntryStage(VelosioJournalEntryBatch velosioJournalEntryStage);
    }
}
