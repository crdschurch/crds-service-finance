using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IPushpayService
    {
        List<PaymentDto> GetDonationsForSettlement(string settlementKey);
        Task<DonationDto> UpdateDonationDetailsFromPushpay(PushpayWebhook webhook);
        void UpdateDonationDetails(PushpayWebhook webhook);
        Task<List<SettlementEventDto>> GetDepositsByDateRange(DateTime startDate, DateTime endDate);
        Task<RecurringGiftDto> CreateRecurringGift(PushpayWebhook webhook, int? congregationId);
        Task<RecurringGiftDto> UpdateRecurringGift(PushpayWebhook webhook, int? congregationId);
        List<PushpayRecurringGiftDto> GetRecurringGiftsByDateRange(DateTime startDate, DateTime endDate);
        Task<MpRecurringGift> BuildAndCreateNewRecurringGift(PushpayRecurringGiftDto pushpayRecurringGift, int? congregationId);
        Task<RecurringGiftDto> UpdateRecurringGiftForSync(PushpayRecurringGiftDto pushpayRecurringGift, MpRecurringGift mpRecurringGift);
        string GetRecurringGiftNotes(PushpayRecurringGiftDto pushpayRecurringGift);
        string FormatPhoneNumber(string phone);
        void SaveWebhookData(PushpayWebhook pushpayWebhook);
        Task<int> LookupCongregationId(List<PushpayFieldValueDto> pushpayFields, string campusKey, int? congregationId);
    }
}
