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
        void SyncDeposits(string hostName);
        List<SettlementEventDto> GetDepositsForSync(DateTime startDate, DateTime endDate);
        void SubmitDeposits(List<SettlementEventDto> deposits, string hostName);
    }
}
