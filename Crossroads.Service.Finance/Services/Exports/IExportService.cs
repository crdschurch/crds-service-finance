using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services.Exports
{
    public interface IExportService
    {
        void CreateJournalEntries();
        string HelloWorld();
        void ExportJournalEntries();
        string ExportJournalEntriesManually(bool markExported);
    }
}
