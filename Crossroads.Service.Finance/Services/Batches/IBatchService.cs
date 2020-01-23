using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IBatchService
    {
        Task<DonationBatchDto> BuildDonationBatch(List<PaymentDto> charges, string depositName, DateTime eventTimestamp, string transferKey);
        Task<DonationBatchDto> SaveDonationBatch(DonationBatchDto donationBatchDto);
        void UpdateDonationBatch(DonationBatchDto donationBatchDto);
    }
}
