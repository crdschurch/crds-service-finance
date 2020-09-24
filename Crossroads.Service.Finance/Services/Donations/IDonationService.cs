using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IDonationService
    {
        List<DonationDto> SetDonationStatus(List<DonationDto> donations, int batchId);
        Task<DonationDto> Update(DonationDto donation);
        Task<List<DonationDto>> Update(List<DonationDto> donations);
        Task<DonationDto> GetDonationByTransactionCode(string transactionCode);
        Task<MpDonor> CreateDonor(MpDonor donor);
        Task<MpDonorAccount> CreateDonorAccount(MpDonorAccount donor);
        Task<MpDonorAccount> FindDonorAccount(PushpayTransactionBaseDto gift, int donorId);
        Task<List<MpDonorAccount>> GetDonorAccounts(int donorId);
        void UpdateDonorAccount(JObject donorAccount);
        Task<List<RecurringGiftDto>> GetRecurringGifts(int contactId);
        Task<List<RecurringGiftDto>> GetRelatedContactRecurringGifts(int userContactId, int relatedContactId);
        Task<List<PledgeDto>> GetPledges(int contactId);
        Task<List<DonationDetailDto>> GetDonations(int contactId);
        Task<List<DonationDetailDto>> GetDonations(string token);
        Task<List<DonationDetailDto>> GetOtherGifts(int contactId);
        Task<List<DonationDetailDto>> GetRelatedContactOtherGifts(int userContactId, int relatedContactId);
        Task<List<MpPledge>> CalculatePledges(int contactId);
        Task<List<DonationDetailDto>> GetRelatedContactDonations(int userContactId, int relatedContactId);
        Task<List<PledgeDto>> GetRelatedContactPledge(int userContactId, int relatedContactId);
        Task<List<DonationDto>> GetDonationsByTransactionCodes(List<string> transactionIds);
        Task<MpDonorAccount> CreateDonorAccountFromPushpay(PushpayTransactionBaseDto gift, int donorId);
        Task UpdateMpDonation(MpDonation mpDonation);
    }
}
