using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
        List<PledgeDto> GetPledges(int contactId);
        List<DonationDetailDto> GetDonations(int contactId);
        List<DonationDetailDto> GetDonations(string token);
        List<DonationDetailDto> GetOtherGifts(int contactId);
        List<MpPledge> CalculatePledges(int contactId);
        List<DonationDetailDto> GetRelatedContactDonations(int userContactId, int relatedContactId);
        List<PledgeDto> GetRelatedContactPledge(int userContactId, int relatedContactId);
    }
}
