using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Exports.Models;

namespace Exports.JournalEntries
{
    public interface IJournalEntryExport
    {
        Task<string> HelloWorld();
        Task<string> ExportJournalEntryStage(string batchNumber, decimal totalDebits, decimal totalCredits,
            int transactionCount, XElement batchData);
    }
}
