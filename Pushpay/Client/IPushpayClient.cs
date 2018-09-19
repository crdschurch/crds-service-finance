using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;
using Pushpay.Models;

namespace Pushpay.Client
{
    public interface IPushpayClient
    {
        PushpayPaymentDto GetPayment(PushpayWebhook webhook);
        PushpayRecurringGiftDto GetRecurringGift(string resource);
        List<PushpayPaymentDto> GetDonations(string settlementKey);
        List<PushpaySettlementDto> GetDepositsByDateRange(DateTime startDate, DateTime endDate);
        List<PushpayRecurringGiftDto> GetRecurringGiftsByDateRange(DateTime startDate, DateTime endDate);
    }
}
