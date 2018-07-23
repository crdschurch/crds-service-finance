using System;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Crossroads.Service.Finance.Controllers;
using Newtonsoft.Json.Linq;
using Pushpay.Models;

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
        private readonly string PushpayMerchantKey = Environment.GetEnvironmentVariable("PUSHPAY_MERCHANT_KEY");

        public PaymentEventService(IBatchService batchService, IDepositService depositService, IDonationService donationService,
            IPushpayService pushpayService)
        {
            _batchService = batchService;
            _depositService = depositService;
            _donationService = donationService;
            _pushpayService = pushpayService;
        }

        public void CreateDeposit(SettlementEventDto settlementEventDto)
        {
            Console.WriteLine($"Creating deposit: {settlementEventDto.Key}");

            // 1. Check to see if the deposit has already been created.  If we do throw an exception.
            var existingDeposit = _depositService.GetDepositByProcessorTransferId(settlementEventDto.Key);
            if (existingDeposit != null)
            {
                //_logger.LogError($"Deposit already exists for settlement: {settlementEventDto.Key}");
                Console.WriteLine($"Deposit already exists for settlement: {settlementEventDto.Key}");
                return;
            }

            // 2. Get all payments associated with a settlement from Pushpay's API. Throw an exception
            // if none are found.
            var settlementPayments = _pushpayService.GetChargesForTransfer(settlementEventDto.Key);
            if (settlementPayments.items == null || settlementPayments.items.Count <= 0)
            {
                //_logger.LogError($"No charges found for settlement: {settlementEventDto.Key}");
                Console.WriteLine($"No charges found for settlement: {settlementEventDto.Key}");
                return;
            }

            // 3. Create and Save the Batch to MP.
            var donationBatch = _batchService.CreateDonationBatch(settlementPayments.items, settlementEventDto.Name,
                DateTime.Now, settlementEventDto.Key);
            var savedDonationBatch = _batchService.SaveDonationBatch(donationBatch);
            donationBatch.Id = savedDonationBatch.Id;
            Console.WriteLine($"Batch created: {savedDonationBatch.Id}");

            // 4. Update all the donations to have a status of deposited and to be part of the new batch.
            var updateDonations = _donationService.SetDonationStatus(donationBatch.Donations, donationBatch.Id);
            _donationService.Update(updateDonations);
            Console.WriteLine($"Updated donations for batch: {donationBatch.Id}");

            // 5. Create Deposit with the associated batch (should be one batch for one deposit)
            var deposit = _depositService.CreateDeposit(settlementEventDto, settlementEventDto.Name, settlementEventDto.Key);
            deposit = _depositService.SaveDeposit(deposit);
            Console.WriteLine($"Deposit created: {deposit.Id}");

            // 6. Update batch with deposit id and name and resave
            donationBatch.DepositId = deposit.Id;
            donationBatch.BatchName = deposit.DepositName + "D";
            _batchService.UpdateDonationBatch(donationBatch);
        }

        public PushpayAnticipatedPaymentDto CreateAnticipatedPayment(PushpayAnticipatedPaymentDto anticipatedPaymentDto)
        {
            // TODO replace when frontend is sending up data
            var samplePushpayAnticipatedPayment = new PushpayAnticipatedPaymentDto()
            {
                Description = "2018 Jul NOLA Trip",
                DescriptionTitle = "Trip Donation For",
                ReturnUrl = "https://www.espn.com",
                ReturnTitle = "Return to espn.com...",
                MerchantKey = PushpayMerchantKey,
                Fields = new List<PushpayAnticipatedPaymentField>
                {
                    new PushpayAnticipatedPaymentField()
                    {
                        Key = "amount",
                        Value =  new JObject(
                            new JProperty("amount", "140.50"),
                            new JProperty("currency", "USD")
                        ),
                        ReadOnly = true
                    },
                    new PushpayAnticipatedPaymentField()
                    {
                        Key = "fund",
                        Value =  "2018 Jul NOLA Trip",
                        ReadOnly = true
                    }
                }
            };
            return _pushpayService.CreateAnticipatedPayment(samplePushpayAnticipatedPayment);
        }
    }
}
