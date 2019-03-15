using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Interfaces
{
    public interface IDonationRepository
    {
        MpDonation GetDonationByTransactionCode(string transactionCode); // theoretically on settlement as transactionid
        List<MpDonation> Update(List<MpDonation> donations);
        MpDonation Update(MpDonation donation);
        MpDonor CreateDonor(MpDonor donor);
        MpDonorAccount CreateDonorAccount(MpDonorAccount donor);
        void UpdateDonorAccount(JObject donorAccount);
        MpContactDonor GetContactDonor(int contactId);
        List<MpRecurringGift> GetRecurringGifts(int contactId);
        List<MpRecurringGift> GetRecurringGiftsByContactIdAndDates(int contactId, DateTime? startDate = null, DateTime? endDate = null);
        //List<MpPledge> GetPledges(string token);
        List<MpDonation> GetDonations(int contactId);
        List<MpDonationDetail> GetDonationHistoryByContactId(int contactId, DateTime? startDate = null, DateTime? endDate = null);
        List<MpDonationDetail> GetOtherGiftsByContactId(int contactId);
        List<MpDonationDetail> GetOtherGiftsForRelatedContact(int contactId, DateTime? startDate = null, DateTime? endDate = null);
    }
}
