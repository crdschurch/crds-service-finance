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

        // TODO: Determine if we need to return anything from this function or if it can be void
        public void CreateDeposit(SettlementEventDto settlementEventDto)
        {
            // TODO: Verify logger is working once we get to testing
            //_logger.LogInformation($"Processing transfer.paid event for transfer id: {settlementEventDto.Key}");

            // Don't process this transfer if we already have a deposit for the same transfer id
            var existingDeposit = _depositService.GetDepositByProcessorTransferId(settlementEventDto.Key);
            if (existingDeposit != null)
            {
                //_logger.LogError($"Deposit already exists for settlement: {settlementEventDto.Key}");
                throw new Exception($"Deposit already exists for settlement: {settlementEventDto.Key}");
            }

            //// once we have a payment being transferred, we need to go call out to Pushpay and 
            //// have to get all payments associated with a settlement
            // var settlementPayments = _pushpayService.GetChargesForTransfer(settlementEventDto.Key);
            //TODO:: will use the above line but currently just stubbing out a answer
            var settlementPayments = new PaymentsDto
            {
                Payments = new List<PaymentProcessorChargeDto> {
                    new PaymentProcessorChargeDto{
                        Status = "Success",
                        TransactionId = "8416544",
                        PaymentToken = "5dcdaPH0wkqvnd4LmB7dEg",
                        Amount = new AmountDto {
                            Currency = "USD",
                            Amount = "22.00"
                        },
                        Source = "WebGiving"
                    },
                    new PaymentProcessorChargeDto{
                        Status = "Success",
                        TransactionId = "8837299",
                        PaymentToken = "5dcdaPH0wkqvnd4LmB7dEgasfdfaew",
                        Amount = new AmountDto {
                            Currency = "USD",
                            Amount = "55.00"
                        },
                        Source = "WebGiving"
                    },
                }
            };

            //// Throw exception if no payments are found for a settlement
            if (settlementPayments.Payments == null || settlementPayments.Payments.Count <= 0)
            {
                //_logger.LogError($"No charges found for settlement: {settlementEventDto.Key}");
                throw new Exception($"No charges found for settlement: {settlementEventDto.Key}");
            }

            var depositName = DateTime.Now.ToString(BatchNameDateFormat);

            var donationBatch = _batchService.CreateDonationBatch(settlementPayments.Payments, depositName + "D",
                DateTime.Now, settlementEventDto.Key);
            donationBatch = _batchService.SaveDonationBatch(donationBatch);

            //// TODO: Call into Donation Service and update Donation Statuses and Assign Batch ID
            var updateDonations = _donationService.SetDonationStatus(donationBatch.Donations, donationBatch.Id);
            _donationService.UpdateDonations(updateDonations);

            //// steps to do:
            //// 4. Create Deposit with the associated batch (should be one batch for one deposit)
            var deposit = _depositService.CreateDeposit(settlementEventDto, depositName);
            deposit = _depositService.SaveDeposit(deposit);

            //// 5. Update batch with deposit id and resave
            donationBatch.DepositId = deposit.Id;
            _batchService.UpdateDonationBatch(donationBatch);
        }
    }
}
