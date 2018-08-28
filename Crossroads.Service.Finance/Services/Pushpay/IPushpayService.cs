using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IPushpayService
    {
        PaymentsDto GetChargesForTransfer(string settlementKey);
        DonationDto UpdateDonationDetailsFromPushpay(PushpayWebhook webhook, bool retry = false);
        void AddUpdateDonationDetailsJob(PushpayWebhook webhook);
        List<SettlementEventDto> GetDepositsByDateRange(DateTime startDate, DateTime endDate);
        PushpayAnticipatedPaymentDto CreateAnticipatedPayment(PushpayAnticipatedPaymentDto anticipatedPayment);
        RecurringGiftDto CreateRecurringGift(PushpayWebhook webhook);
        RecurringGiftDto UpdateRecurringGift(PushpayWebhook webhook);
    }
}
