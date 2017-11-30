using System;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Services.Donations;

namespace Crossroads.Service.Finance.Services.PaymentEvents
{
    public class PaymentEventService
    {
        private readonly IDonationService _donationService;

        public PaymentEventService(IDonationService donationService)
        {
            _donationService = donationService;
        }

        public PaymentEventResponseDto CreateDeposit(SettlementEventDto settlementEventDto)
        {
            // TODO: Add logger
            //_logger.Debug("Processing transfer.paid event for transfer id " + transfer.Id);

            var response = new TransferPaidResponseDto();

            // Don't process this transfer if we already have a deposit for the same transfer id
            var existingDeposit = _donationService.GetDepositByProcessorTransferId(settlementEventDto.Key);
            if (existingDeposit != null)
            {
                var msg = $"Deposit {existingDeposit.Id} already created for transfer {existingDeposit.ProcessorTransferId}";
                //_logger.Debug(msg); TODO: Add
                response.TotalTransactionCount = 0;
                response.Message = msg;
                response.Exception = new ApplicationException(msg);
                return response;
            }

            // once we have a payment being transferred, we need to go call out to Pushpay and 
            // have to get donations associated with that deposit

            // steps to do:
            // 2. GetDonationsForDesposit (was GetChargesForTransfer)
            // 3. Create Batch (if needed - the batch creation, not the code, I mean)
            // 4. Create Deposit with the associated batch (should be one batch for one deposit)
        }
    }
}
