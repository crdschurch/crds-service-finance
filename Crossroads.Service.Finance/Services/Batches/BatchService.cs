using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Services.Batches
{
    public class BatchService
    {
        public DonationBatchDto CreateDonationBatch(List<PaymentProcessorChargeDto> charges, string depositName, 
            DateTime eventTimestamp, string transferKey)
        {
            var now = DateTime.Now;

            var batch = new DonationBatchDto()
            {
                Id = 1234567,
                BatchName = depositName,
                SetupDateTime = now,
                BatchTotalAmount = 0,
                ItemCount = 0,
                BatchEntryType = 10, // hardcoded now, comes from config value
                FinalizedDateTime = now,
                DepositId = null,
                ProcessorTransferId = transferKey
            };

            // TODO:
            // 1. Loops through the charges to get corresponding DonationDTO and set that on the batch

            // 2. Add the charge amount to the batch total amount -
            //      batch.ItemCount++;
            //      batch.BatchTotalAmount += (charge.Amount / Constants.StripeDecimalConversionValue);
            //      batch.Payments.Add(new PaymentDTO { PaymentId = payment.PaymentId, Amount = charge.Amount, ProcessorFee = charge.Fee, BatchId = payment.BatchId, ContactId = payment.ContactId, InvoiceId = payment.InvoiceId, StripeTransactionId = payment.StripeTransactionId });
            //      batch.BatchFeeTotal = batch.Payments.Sum(f => f.ProcessorFee);

            // 3. Implement save of the batch here - down to the repo and get the object back with an id on it

            return batch;
        }
    }
}
