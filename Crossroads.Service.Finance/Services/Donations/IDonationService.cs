using System.Collections.Generic;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;

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
    }
}
