using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IDepositService
    {
        Task<DepositDto> BuildDeposit(SettlementEventDto settlementEventDto);
        Task<DepositDto> SaveDeposit(DepositDto depositDto);
        Task<DepositDto> GetDepositByProcessorTransferId(string key);
        Task<List<SettlementEventDto>> SyncDeposits();
        Task<List<SettlementEventDto>> GetDepositsForSync(DateTime startDate, DateTime endDate);
    }
}
