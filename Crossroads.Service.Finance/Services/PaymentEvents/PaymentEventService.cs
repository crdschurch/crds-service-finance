using System;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Services.Batches;
using Crossroads.Service.Finance.Services.Deposits;
using Crossroads.Service.Finance.Services.Donations;
using Crossroads.Service.Finance.Services.PaymentProcessor;

namespace Crossroads.Service.Finance.Services.PaymentEvents
{
    public class PaymentEventService
    {
        private readonly IBatchService _batchService;
        private readonly IDonationService _donationService;
        private readonly IDepositService _depositService;
        private readonly IPaymentProcessorService _paymentProcessorService;

        // This value is used when creating the batch name for exporting to GP.  It must be 15 characters or less.
        private const string BatchNameDateFormat = @"\M\PyyMMddHHmmss";

        public PaymentEventService(IBatchService batchService, IDonationService donationService, IPaymentProcessorService paymentProcessorService)
        {
            _batchService = batchService;
            _donationService = donationService;
            _paymentProcessorService = paymentProcessorService;
        }

        // TODO: Determine if we need to return anything from this function or if it can be void
        public PaymentEventResponseDto CreateDeposit(SettlementEventDto settlementEventDto)
        {
            // TODO: Add logger
            //_logger.Debug("Processing transfer.paid event for transfer id " + transfer.Id);

            var response = new TransferPaidResponseDto();

            // Don't process this transfer if we already have a deposit for the same transfer id
            var existingDeposit = _donationService.GetDepositByProcessorTransferId(settlementEventDto.Key);
            if (existingDeposit != null)
            {
                //var msg = $"Deposit {existingDeposit.Id} already created for transfer {existingDeposit.ProcessorTransferId}";
                ////_logger.Debug(msg); TODO: Add
                //response.TotalTransactionCount = 0;
                //response.Message = msg;
                //response.Exception = new ApplicationException(msg);
                //return response;

                // TODO: Add logging for this, as it's an exception case
                return null;
            }



            // once we have a payment being transferred, we need to go call out to Pushpay and 
            // have to get all payments associated with a settlement
            var settlementPayments = _paymentProcessorService.GetChargesForTransfer(settlementEventDto.Key);

            // TODO: Switch to logging and return null or exception case - this should theoretically never occur
            if (settlementPayments.payments == null || settlementPayments.payments.Count <= 0)
            {
                var msg = "No charges found for settlement: " + settlementEventDto.Key;
                //_logger.Debug(msg);
                response.TotalTransactionCount = 0;
                response.Message = msg;
                response.Exception = new ApplicationException(msg);
                return (response);
            }

            var depositName = DateTime.Now.ToString(BatchNameDateFormat);

            var donationBatch = _batchService.CreateDonationBatch(settlementPayments.payments, depositName + "D",
                System.DateTime.Now,
                settlementEventDto.Key);

            // TODO: Verify we're saving the donation batch - consider pulling it out to a separate function which only saves it
            // in the batch service/repo
            donationBatch = _batchService.SaveDonationBatch(donationBatch);

            // TODO: Call into Donation Service and update Donation Statuses and Assign Batch ID
            var updateDonations = _donationService.UpdateDonationStatus(donationBatch.Donations, donationBatch.Id);
            _donationService.SaveDonations(updateDonations);

            // steps to do:
            // 4. Create Deposit with the associated batch (should be one batch for one deposit)
            var deposit = _depositService.CreateDeposit(settlementEventDto, depositName);
            deposit = _depositService.SaveDeposit(deposit);

            // 5. Update batch with deposit id and resave
            donationBatch.DepositId = deposit.Id;
            _batchService.UpdateDonationBatch(donationBatch);

            return null;
        }
    }
}
