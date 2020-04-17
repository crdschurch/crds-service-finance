using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLogging.Transfer
{
    public interface ITransferData
    {
        // TODO: Test method, remove at later date
        Task<List<string>> DisplayDatabaseNames();
    }
}
