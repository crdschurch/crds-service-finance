using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Services.Batches
{
    public interface IBatchService
    {
        DonationBatchDto CreateDonationBatch(List<PaymentProcessorChargeDto> charges, string depositName, DateTime eventTimestamp, string transferKey);
        DonationBatchDto SaveDonationBatch(DonationBatchDto donationBatchDto);
        void UpdateDonationBatch(DonationBatchDto donationBatchDto);
    }
}
