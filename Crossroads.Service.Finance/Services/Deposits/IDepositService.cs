using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IDepositService
    {
        DepositDto CreateDeposit(SettlementEventDto settlementEventDto, string depositName);
        DepositDto SaveDeposit(DepositDto depositDto);
        DepositDto GetDepositByProcessorTransferId(string key);
        void SyncDeposits();
        List<DepositDto> GetDepositsForSync(DateTime startDate, DateTime endDate);
        void SyncDeposits(List<DepositDto> deposits);
        List<DepositDto> GetDepositsByTransferIds(List<string> transferIds);
    }
}
