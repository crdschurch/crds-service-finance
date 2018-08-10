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
        //List<MpPledge> GetPledges(string token);
        List<MpDonation> GetDonations(int contactId);
        List<MpDonationHistory> GetDonationHistoryByContactId(int contactId, DateTime? startDate = null, DateTime? endDate = null);
    }
}
