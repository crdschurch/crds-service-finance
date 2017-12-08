using System;
using System.Collections.Generic;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;

namespace Crossroads.Service.Finance.Services
{
    public class BatchService : IBatchService
    {
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
        public DonationBatchDto CreateDonationBatch(List<PaymentProcessorChargeDto> charges, string depositName, 
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
                var mpDonation = _donationRepository.GetDonationByTransactionCode(charge.TransactionId);

                // Add the charge amount to the batch total amount
                batch.ItemCount++;
                batch.BatchTotalAmount += decimal.Parse(charge.Amount.Amount);
                batch.Donations.Add(_mapper.Map<DonationDto>(mpDonation));
            }

            return batch;
        }

        public DonationBatchDto SaveDonationBatch(DonationBatchDto donationBatchDto)
        {
            var mpBatch = _batchRepository.CreateDonationBatch(_mapper.Map<MpDonationBatch>(donationBatchDto));
            return _mapper.Map<DonationBatchDto>(mpBatch);
        }

        public void UpdateDonationBatch(DonationBatchDto donationBatchDto)
        {
            _batchRepository.UpdateDonationBatch(_mapper.Map<MpDonationBatch>(donationBatchDto));
        }
    }
}
