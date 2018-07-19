using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IDonationService
    {
        List<DonationDto> SetDonationStatus(List<DonationDto> donations, int batchId);
        DonationDto Update(DonationDto donation);
        List<DonationDto> Update(List<DonationDto> donations);
        DonationDto GetDonationByTransactionCode(string transactionCode);
        MpDonor CreateDonor(MpDonor donor);
        MpDonorAccount CreateDonorAccount(MpDonorAccount donor);
        void UpdateDonorAccount(JObject donorAccount);
        List<RecurringGiftDto> GetRecurringGifts(string token);
        List<PledgeDto> GetPledges(string token);
        List<DonationDto> GetDonations(string token);
        List<MpPledge> CalculatePledges(string token);
        List<DonationHistoryDto> GetDonationHistoryByContactId(int contactId, string token);
    }
}
