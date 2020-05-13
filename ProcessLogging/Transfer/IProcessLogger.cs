using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProcessLogging.Models;

namespace ProcessLogging.Transfer
{
    public interface IProcessLogger
    {
        void SaveProcessLogMessage(ProcessLogMessage processLogMessage);
    }
}
