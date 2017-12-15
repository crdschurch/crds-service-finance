using System;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Crossroads.Service.Finance.Services
{
    public class PaymentEventService : IPaymentEventService
    {
        private readonly IBatchService _batchService;
        private readonly IDepositService _depositService;
        private readonly IDonationService _donationService;
        //private readonly ILogger _logger;
        private readonly IPushpayService _pushpayService;
        
        // This value is used when creating the batch name for exporting to GP.  It must be 15 characters or less.
        private const string BatchNameDateFormat = @"\M\PyyMMddHHmmss";

        public PaymentEventService(IBatchService batchService, IDepositService depositService, IDonationService donationService,
            IPushpayService pushpayService)
        {
            _batchService = batchService;
            _depositService = depositService;
            _donationService = donationService;
            _pushpayService = pushpayService;
            //_logger = logger;
        }

        public void CreateDeposit(SettlementEventDto settlementEventDto)
        {
            // TODO: Verify logger is working once we get to testing
            //_logger.LogInformation($"Processing transfer.paid event for transfer id: {settlementEventDto.Key}");

            // 1. Check to see if the deposit has already been created.  If we do throw an exception.
            var existingDeposit = _depositService.GetDepositByProcessorTransferId(settlementEventDto.Key);
            if (existingDeposit != null)
            {
                //_logger.LogError($"Deposit already exists for settlement: {settlementEventDto.Key}");
                throw new Exception($"Deposit already exists for settlement: {settlementEventDto.Key}");
            }

            // 2. Get all payments associated with a settlement from Pushpay's API. Throw an exception
            // if none are found.
            var settlementPayments = _pushpayService.GetChargesForTransfer(settlementEventDto.Key);
            if (settlementPayments.items == null || settlementPayments.items.Count <= 0)
            {
                //_logger.LogError($"No charges found for settlement: {settlementEventDto.Key}");
                throw new Exception($"No charges found for settlement: {settlementEventDto.Key}");
            }

            // 3. Generate the Deposit Name.
            var depositName = DateTime.Now.ToString(BatchNameDateFormat);

            // 4. Create and Save the Batch to MP.
            var donationBatch = _batchService.CreateDonationBatch(settlementPayments.items, depositName + "D",
                DateTime.Now, settlementEventDto.Key);
            var savedDonationBatch = _batchService.SaveDonationBatch(donationBatch);
            donationBatch.Id = savedDonationBatch.Id;

            try
            {
                // 5. Update all the donations to have a status of deposited and to be part of the new batch.
                var updateDonations = _donationService.SetDonationStatus(donationBatch.Donations, donationBatch.Id);
                _donationService.UpdateDonations(updateDonations);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }


            // 6. Create Deposit with the associated batch (should be one batch for one deposit)
            var deposit = _depositService.CreateDeposit(settlementEventDto, depositName);
            deposit = _depositService.SaveDeposit(deposit);

            // 7. Update batch with deposit id and resave
            donationBatch.DepositId = deposit.Id;
            _batchService.UpdateDonationBatch(donationBatch);
        }
    }
}
