using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IDepositRepository
    {
        Task<MpDeposit> CreateDeposit(MpDeposit mpDeposit);
        Task<MpDeposit> GetDepositByProcessorTransferId(string processorTransferId);
        Task<List<MpDeposit>> GetByTransferIds(List<string> transferIds);
        Task<List<MpDeposit>> GetByName(string DepositName);
    }
}
