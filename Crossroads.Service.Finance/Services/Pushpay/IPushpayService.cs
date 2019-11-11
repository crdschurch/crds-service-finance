﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IPushpayService
    {
        Task<List<PaymentDto>> GetDonationsForSettlement(string settlementKey);
        Task<DonationDto> UpdateDonationDetailsFromPushpay(PushpayWebhook webhook, bool retry = false);
        void UpdateDonationDetails(PushpayWebhook webhook);
        Task<List<SettlementEventDto>> GetDepositsByDateRange(DateTime startDate, DateTime endDate);
        Task<RecurringGiftDto> CreateRecurringGift(PushpayWebhook webhook);
        Task<RecurringGiftDto> UpdateRecurringGift(PushpayWebhook webhook);
        Task<List<PushpayRecurringGiftDto>> GetRecurringGiftsByDateRange(DateTime startDate, DateTime endDate);
        Task<MpRecurringGift> BuildAndCreateNewRecurringGift(PushpayRecurringGiftDto pushpayRecurringGift);
        Task<RecurringGiftDto> UpdateRecurringGiftForSync(PushpayRecurringGiftDto pushpayRecurringGift, MpRecurringGift mpRecurringGift);
        string GetRecurringGiftNotes(PushpayRecurringGiftDto pushpayRecurringGift);
        string FormatPhoneNumber(string phone);
        void SaveWebhookData(PushpayWebhook pushpayWebhook);
    }
}
