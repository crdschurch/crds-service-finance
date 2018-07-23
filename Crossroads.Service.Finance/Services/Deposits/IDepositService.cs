using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IDepositService
    {
        DepositDto CreateDeposit(SettlementEventDto settlementEventDto);
        DepositDto SaveDeposit(DepositDto depositDto);
        DepositDto GetDepositByProcessorTransferId(string key);
        List<SettlementEventDto> SyncDeposits();
        List<SettlementEventDto> GetDepositsForSync(DateTime startDate, DateTime endDate);
    }
}
