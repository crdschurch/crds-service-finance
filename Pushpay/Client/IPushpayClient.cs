using System;
using System.Collections.Generic;
using Pushpay.Models;

namespace Pushpay.Client
{
    public interface IPushpayClient
    {
        PushpayPaymentsDto GetPushpayDonations(string settlementKey);
        List<PushpayDepositDto> GetDepositByDateRange(DateTime startDate, DateTime endDate);
    }
}
