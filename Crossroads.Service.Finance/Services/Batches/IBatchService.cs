using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IBatchService
    {
        DonationBatchDto BuildDonationBatch(List<PaymentDto> charges, string depositName, DateTime eventTimestamp, string transferKey);
        DonationBatchDto SaveDonationBatch(DonationBatchDto donationBatchDto);
        void UpdateDonationBatch(DonationBatchDto donationBatchDto);
    }
}
