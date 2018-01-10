using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;
using Pushpay.Models;

namespace Pushpay.Client
{
    public interface IPushpayClient
    {
        PushpayPaymentsDto GetPushpayDonations(string settlementKey);
        PushpayPaymentDto GetPayment(PushpayWebhook webhook);
        List<PushpaySettlementDto> GetDepositsByDateRange(DateTime startDate, DateTime endDate);
        PushpayAnticipatedPaymentDto CreateAnticipatedPayment(PushpayAnticipatedPaymentDto anticipatedPayment);
    }
}
