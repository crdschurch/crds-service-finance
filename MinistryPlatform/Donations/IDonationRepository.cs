using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Interfaces
{
    public interface IDonationRepository
    {
        Task<MpDonation> GetDonationByTransactionCode(string transactionCode); // theoretically on settlement as transactionid
        Task<List<MpDonation>> Update(List<MpDonation> donations);
        Task<MpDonation> Update(MpDonation donation);
        Task<MpDonor> CreateDonor(MpDonor donor);
        Task<MpDonorAccount> CreateDonorAccount(MpDonorAccount donor);
        Task<List<MpDonorAccount>> GetDonorAccounts(int donorId);
        void UpdateDonorAccount(JObject donorAccount);
        Task<List<MpRecurringGift>> GetRecurringGifts(int contactId);
        Task<List<MpRecurringGift>> GetRecurringGiftsByContactIdAndDates(int contactId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<MpDonation>> GetDonations(int contactId);
        Task<List<MpDonationDetail>> GetDonationHistoryByContactId(int contactId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<MpDonationDetail>> GetOtherGiftsByContactId(int contactId);
        Task<List<MpDonationDetail>> GetOtherGiftsForRelatedContact(int contactId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<MpDonation>> GetDonationsByTransactionIds(List<string> transactionIds);
        void CreateRawPushpayDonation(string rawRecurringGiftSchedule);
        Task<List<MpRawDonation>> GetUnprocessedDonations(int? lastSyncIndex = null);
        Task BatchMarkAsProcessed(List<int> ids);
    }
}
