using System;
using System.Collections.Generic;
using Pushpay.Models;

namespace Pushpay.Client
{
    public interface IPushpayClient
    {
        PushpayPaymentsDto GetPushpayDonations(string settlementKey);
        List<PushpaySettlementDto> GetDepositsByDateRange(DateTime startDate, DateTime endDate);
    }
}
