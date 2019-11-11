﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;
using Pushpay.Models;

namespace Pushpay.Client
{
    public interface IPushpayClient
    {
        Task<PushpayPaymentDto> GetPayment(PushpayWebhook webhook);
        Task<PushpayRecurringGiftDto> GetRecurringGift(string resource);
        Task<List<PushpayPaymentDto>> GetDonations(string settlementKey);
        Task<List<PushpaySettlementDto>> GetDepositsByDateRange(DateTime startDate, DateTime endDate);
        Task<List<PushpayRecurringGiftDto>> GetNewAndUpdatedRecurringGiftsByDateRange(DateTime startDate, DateTime endDate);
    }
}
