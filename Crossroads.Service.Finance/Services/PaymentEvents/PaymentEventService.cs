using System;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Services.Batches;
using Crossroads.Service.Finance.Services.Deposits;
using Crossroads.Service.Finance.Services.Donations;
using Crossroads.Service.Finance.Services.PaymentProcessor;
using Microsoft.Extensions.Logging;

namespace Crossroads.Service.Finance.Services.PaymentEvents
{
    public class PaymentEventService : IPaymentEventService
    {
        private readonly IBatchService _batchService;
        private readonly IDepositService _depositService;
        private readonly IDonationService _donationService;
        private readonly ILogger _logger;
        private readonly IPaymentProcessorService _paymentProcessorService;
        
        // This value is used when creating the batch name for exporting to GP.  It must be 15 characters or less.
        private const string BatchNameDateFormat = @"\M\PyyMMddHHmmss";

        public PaymentEventService(IBatchService batchService, IDepositService depositService, IDonationService donationService, 
            IPaymentProcessorService paymentProcessorService)
        {
            _batchService = batchService;
            _depositService = depositService;
            _donationService = donationService;
            _paymentProcessorService = paymentProcessorService;
        }

        // TODO: Determine if we need to return anything from this function or if it can be void
        public void CreateDeposit(SettlementEventDto settlementEventDto)
        {
            // TODO: Verify logger is working once we get to testing
            _logger.LogInformation($"Processing transfer.paid event for transfer id: {settlementEventDto.Key}");

            // Don't process this transfer if we already have a deposit for the same transfer id
            var existingDeposit = _depositService.GetDepositByProcessorTransferId(settlementEventDto.Key);
            if (existingDeposit != null)
            {
                _logger.LogError($"Deposit already exists for settlement: {settlementEventDto.Key}");
                throw new Exception($"Deposit already exists for settlement: {settlementEventDto.Key}");
            }

            // once we have a payment being transferred, we need to go call out to Pushpay and 
            // have to get all payments associated with a settlement
            var settlementPayments = _paymentProcessorService.GetChargesForTransfer(settlementEventDto.Key);

            // Throw exception if no payments are found for a settlement
            if (settlementPayments.payments == null || settlementPayments.payments.Count <= 0)
            {
                _logger.LogError($"No charges found for settlement: {settlementEventDto.Key}");
                throw new Exception($"No charges found for settlement: {settlementEventDto.Key}");
            }

            var depositName = DateTime.Now.ToString(BatchNameDateFormat);

            var donationBatch = _batchService.CreateDonationBatch(settlementPayments.payments, depositName + "D",
                DateTime.Now, settlementEventDto.Key);
            donationBatch = _batchService.SaveDonationBatch(donationBatch);

            // TODO: Call into Donation Service and update Donation Statuses and Assign Batch ID
            var updateDonations = _donationService.SetDonationStatus(donationBatch.Donations, donationBatch.Id);
            _donationService.UpdateDonations(updateDonations);

            // steps to do:
            // 4. Create Deposit with the associated batch (should be one batch for one deposit)
            var deposit = _depositService.CreateDeposit(settlementEventDto, depositName);
            deposit = _depositService.SaveDeposit(deposit);

            // 5. Update batch with deposit id and resave
            donationBatch.DepositId = deposit.Id;
            _batchService.UpdateDonationBatch(donationBatch);
        }
    }
}
