using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using System;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services
{
    public class PaymentEventService : IPaymentEventService
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IBatchService _batchService;
        private readonly IDepositService _depositService;
        private readonly IDonationService _donationService;
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
            _logger.Info($"Creating deposit: {settlementEventDto.Key}");

            // 1. Check to see if the deposit has already been created.  If we do throw an exception.
            var existingDeposit = _depositService.GetDepositByProcessorTransferId(settlementEventDto.Key).Result;
            if (existingDeposit != null)
            {
                Console.WriteLine($"Deposit already exists for settlement: {settlementEventDto.Key}");
                _logger.Info($"Deposit already exists for settlement: {settlementEventDto.Key}");
                return;
            }

            // 2. Get all payments associated with a settlement from Pushpay's API. Throw an exception
            // if none are found.
            var settlementPayments = _pushpayService.GetDonationsForSettlement(settlementEventDto.Key);
            Console.WriteLine($"Settlement {settlementEventDto.Key} contains {settlementPayments.Count} ({settlementPayments}) donations from pushpay");

            if (settlementPayments.Count <= 0)
            {
                Console.WriteLine($"No charges found for settlement: {settlementEventDto.Key}");
                _logger.Info($"No charges found for settlement: {settlementEventDto.Key}");
                return;
            }

            // 3. Create and Save the Batch to MP.
            var donationBatch = _batchService.BuildDonationBatch(settlementPayments, settlementEventDto.Name,
                DateTime.Now, settlementEventDto.Key).Result;
            var savedDonationBatch = _batchService.SaveDonationBatch(donationBatch).Result;
            donationBatch.Id = savedDonationBatch.Id;

            _logger.Info($"Batch created: {savedDonationBatch.Id}");
            Console.WriteLine($"Batch created: {savedDonationBatch.Id}");

            // 4. Update all the donations to have a status of deposited and to be part of the new batch.
            var updateDonations = _donationService.SetDonationStatus(donationBatch.Donations, donationBatch.Id);
            _donationService.Update(updateDonations);
            Console.WriteLine($"Updated donations for batch: {donationBatch.Id}");

            _logger.Info($"Updated donations for batch: {donationBatch.Id}");
            Console.WriteLine($"Updated donations for batch: {donationBatch.Id}");

            // 5. Create Deposit with the associated batch (should be one batch for one deposit)
            var deposit = _depositService.BuildDeposit(settlementEventDto).Result;
            deposit = _depositService.SaveDeposit(deposit).Result;

            _logger.Info($"Deposit created: {deposit.Id}");
            Console.WriteLine($"Deposit created: {deposit.Id}");

            // 6. Update batch with deposit id and name and re-save
            donationBatch.DepositId = deposit.Id;
            donationBatch.BatchName = deposit.DepositName + "D";
            _batchService.UpdateDonationBatch(donationBatch);
        }
    }
}
