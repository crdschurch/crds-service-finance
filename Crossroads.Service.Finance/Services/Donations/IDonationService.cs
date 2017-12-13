using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IDonationService
    {
        List<DonationDto> SetDonationStatus(List<DonationDto> donations, int batchId);
        List<DonationDto> UpdateDonations(List<DonationDto> donations);
        DonationDto GetDonationByTransactionCode(string transactionCode);
    }
}
