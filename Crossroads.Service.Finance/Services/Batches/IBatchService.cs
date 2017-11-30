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
        // settlementPayments.payments, depositName + "D",
        //System.DateTime.Now,
        //settlementEventDto.Key
    }
}
