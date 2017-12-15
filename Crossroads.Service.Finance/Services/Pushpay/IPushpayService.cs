using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IPushpayService
    {
        PaymentsDto GetChargesForTransfer(string settlementKey);
        DonationDto UpdateDonationStatusFromPushpay(PushpayWebhook webhook);
        List<SettlementEventDto> GetDepositsByDateRange(DateTime startDate, DateTime endDate);
    }
}
