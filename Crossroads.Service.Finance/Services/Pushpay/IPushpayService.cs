using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IPushpayService
    {
        PaymentsDto GetChargesForTransfer(string settlementKey);
        List<SettlementEventDto> GetDepositsByDateRange(DateTime startDate, DateTime endDate);
    }
}
