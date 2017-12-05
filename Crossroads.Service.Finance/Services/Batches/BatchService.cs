using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;

namespace Crossroads.Service.Finance.Services.Batches
{
    public class BatchService : IBatchService
    {
        private readonly IDonationRepository _donationRepository;
        private readonly IBatchRepository _batchRepository;
        private readonly IMapper _mapper;

        public BatchService(IDonationRepository donationRepository, IBatchRepository batchRepository, IMapper mapper)
        {
            _donationRepository = donationRepository;
            _batchRepository = batchRepository;
            _mapper = mapper;
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
                // TODO:: _configurationWrapper.GetMpConfigIntValue( "BatchEntryTypePaymentProcessor"), hardcoded now, comes from config value
                BatchEntryType = 10,
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

                var donationDto = new DonationDto
                {
                    Id = mpDonation.DonationId.ToString(),
                    Amount = decimal.Parse(charge.Amount.Amount)
                };

                // TODO: We don't want to save the list of donations on this batch - potentially clear out or ignore before save to avoid creating duplicate data
                batch.Donations.Add(donationDto);
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
            
        }
    }
}
