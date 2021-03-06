﻿using Crossroads.Service.Finance.Models;
using Pushpay.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pushpay.Client
{
    public interface IPushpayClient
    {
        Task<PushpayPaymentDto> GetPayment(PushpayWebhook webhook);
        Task<PushpayRecurringGiftDto> GetRecurringGift(string resource);
        List<PushpayPaymentDto> GetDonations(string settlementKey);
        Task<List<PushpaySettlementDto>> GetDepositsByDateRange(DateTime startDate, DateTime endDate);
        List<PushpayRecurringGiftDto> GetNewAndUpdatedRecurringGiftsByDateRange(DateTime startDate, DateTime endDate);
        Task<List<PushpayPaymentDto>> GetPolledDonations(DateTime startTime, DateTime endTime);
        Task<List<string>> GetRecurringGiftsAsync(DateTime startDate, DateTime endDate);
        Task<List<string>> GetPolledDonationsJson(DateTime startTime, DateTime endTime);
    }
}
