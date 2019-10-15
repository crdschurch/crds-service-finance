using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services.Exports
{
    public interface IExportService
    {
        Task CreateJournalEntries();
        Task<string> HelloWorld();
        void ExportJournalEntries();
        Task<string> ExportJournalEntriesManually(bool markExported);
    }
}
