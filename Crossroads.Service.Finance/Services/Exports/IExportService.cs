using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services.Exports
{
    public interface IExportService
    {
        void CreateJournalEntriesAsync();
        Task<string> HelloWorld();
        Task<string> ExportJournalEntries();
        Task<string> ExportJournalEntriesManually(bool markExported);
    }
}