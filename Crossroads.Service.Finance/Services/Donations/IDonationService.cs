using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IDonationService
    {
        List<DonationDto> SetDonationStatus(List<DonationDto> donations, int batchId);
        Task<DonationDto> Update(DonationDto donation);
        List<DonationDto> Update(List<DonationDto> donations);
        Task<DonationDto> GetDonationByTransactionCode(string transactionCode);
        Task<MpDonor> CreateDonor(MpDonor donor);
        Task<MpDonorAccount> CreateDonorAccount(MpDonorAccount donor);
        void UpdateDonorAccount(JObject donorAccount);
        List<RecurringGiftDto> GetRecurringGifts(int contactId);
        List<RecurringGiftDto> GetRelatedContactRecurringGifts(int userContactId, int relatedContactId);
        List<PledgeDto> GetPledges(int contactId);
        List<DonationDetailDto> GetDonations(int contactId);
        List<DonationDetailDto> GetDonations(string token);
        List<DonationDetailDto> GetOtherGifts(int contactId);
        List<DonationDetailDto> GetRelatedContactOtherGifts(int userContactId, int relatedContactId);
        List<MpPledge> CalculatePledges(int contactId);
        List<DonationDetailDto> GetRelatedContactDonations(int userContactId, int relatedContactId);
        List<PledgeDto> GetRelatedContactPledge(int userContactId, int relatedContactId);
    }
}
