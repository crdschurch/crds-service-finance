using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Batches;
using MinistryPlatform.Models;
using MinistryPlatform.Donations;

namespace Crossroads.Service.Finance.Services.Batches
{
    public class BatchService : IBatchService
    {
        private readonly IConfigurationWrapper _configurationWrapper;
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
                Id = 1234567,
                BatchName = depositName,
                SetupDateTime = DateTime.Now,
                BatchTotalAmount = 0,
                ItemCount = 0,
                // TODO:: _configurationWrapper.GetMpConfigIntValue( "BatchEntryTypePaymentProcessor"), hardcoded now, comes from config value
                BatchEntryType = 10,
                FinalizedDateTime = DateTime.Now,
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
            var mpDepostResult = _batchRepository.CreateDonationBatch(_mapper.Map<MpDonationBatch>(donationBatchDto));
            return _mapper.Map<DonationBatchDto>(mpDepostResult);
        }

        public void UpdateDonationBatch(DonationBatchDto donationBatchDto)
        {
            
        }
    }
}
