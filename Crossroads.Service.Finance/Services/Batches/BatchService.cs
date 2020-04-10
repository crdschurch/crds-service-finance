using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services
{
    public class BatchService : IBatchService
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IDonationRepository _donationRepository;
        private readonly IBatchRepository _batchRepository;
        private readonly IMapper _mapper;

        private readonly int _batchEntryTypeValue;

        public BatchService(IDonationRepository donationRepository, IBatchRepository batchRepository, IMapper mapper, IConfigurationWrapper configurationWrapper)
        {
            _donationRepository = donationRepository;
            _batchRepository = batchRepository;
            _mapper = mapper;

            _batchEntryTypeValue = configurationWrapper.GetMpConfigIntValue("CRDS-FINANCE", "BatchEntryType", true).GetValueOrDefault();
        }

        // This function creates the batch in MP, then returns the object so that the deposit can be added to the batch
        public async Task<DonationBatchDto> BuildDonationBatch(List<PaymentDto> charges, string depositName, 
            DateTime eventTimestamp, string transferKey)
        {
            var batch = new DonationBatchDto()
            {
                BatchName = depositName,
                SetupDateTime = eventTimestamp,
                BatchTotalAmount = 0,
                ItemCount = 0,
                BatchEntryType = _batchEntryTypeValue,
                FinalizedDateTime = eventTimestamp,
                DepositId = null,
                ProcessorTransferId = transferKey
            };

            foreach (var charge in charges)
            {
                var mpDonation = await _donationRepository.GetDonationByTransactionCode("PP-" + charge.TransactionId);
                if (mpDonation != null)
                {
                    // Add the charge amount to the batch total amount
                    batch.ItemCount++;
                    // Pushpay amounts are always positive, so check if refund
                    if (charge.IsRefund) {
                        batch.BatchTotalAmount -= decimal.Parse(charge.Amount.Amount);
                    } else {
                        batch.BatchTotalAmount += decimal.Parse(charge.Amount.Amount);
                    }
                    batch.Donations.Add(_mapper.Map<DonationDto>(mpDonation));
                }
                else
                {
                    Console.WriteLine($"Error in BatchService.BuildDonationBatch: Donation not found in MP for transaction code: " +
                                      $"{charge.TransactionId}. Batch total will not match deposit total for batch={depositName}.");
                    _logger.Error($"Error in BatchService.BuildDonationBatch: Donation not found in MP for transaction code: " +
                                  $"{charge.TransactionId}. Batch total will not match deposit total for batch={depositName}.");
                }
            }   

            return batch;
        }

        public async Task<DonationBatchDto> SaveDonationBatch(DonationBatchDto donationBatchDto)
        {
            var mpBatch = await _batchRepository.CreateDonationBatch(_mapper.Map<MpDonationBatch>(donationBatchDto));
            return _mapper.Map<DonationBatchDto>(mpBatch);
        }

        public void UpdateDonationBatch(DonationBatchDto donationBatchDto)
        {
            _batchRepository.UpdateDonationBatch(_mapper.Map<MpDonationBatch>(donationBatchDto));
        }
    }
}
