using System.Threading.Tasks;
using MinistryPlatform.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Services.Donor
{
    public interface IDonorService
    {
        Task<int?> FindDonorId(PushpayTransactionBaseDto pushpayTransactionBaseDto);
        Task<MpDonor> CreateDonor(MpDonor donor);
        Task<MpDonor> CreateDonor(PushpayTransactionBaseDto gift);
    }
}