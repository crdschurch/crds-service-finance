using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Donations;

namespace Crossroads.Service.Finance.Services.Batches
{
    public class BatchService
    {
        private readonly IConfigurationWrapper _configurationWrapper;
        private readonly IDonationRepository _donationRepository;
        private readonly IMapper _mapper;

        public BatchService(IDonationRepository donationRepository, IMapper mapper)
        {
            _donationRepository = donationRepository;
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
                BatchEntryType = 10, //_configurationWrapper.GetMpConfigIntValue( "BatchEntryTypePaymentProcessor"), // hardcoded now, comes from config value
                FinalizedDateTime = DateTime.Now,
                DepositId = null,
                ProcessorTransferId = transferKey
            };

            foreach (var charge in charges)
            {
                var mpDonation = _donationRepository.GetDonationByTransactionCode(charge.TransactionId);
                //var donationDto = _mapper.Map<DonationDto>(mpDonation);

                // Add the charge amount to the batch total amount
                batch.ItemCount++;
                batch.BatchTotalAmount += Decimal.Parse(charge.Amount.Amount);

                var donationDto = new DonationDto
                {
                    Id = mpDonation.donationId.ToString(),
                    Amount = Int32.Parse(charge.Amount.Amount)

                    // TODO: need to figure out how to get the fees onto the batch for
                    // MP to GP conversion
                    //Fee = charge.
                };

                // TODO: We don't want to save the list of donations on this batch - potentially clear out or ignore before
                // save to avoid creating duplicate data
                batch.Donations.Add(donationDto);

                // TODO: need to figure out how to get the fees onto the batch
                //batch.BatchFeeTotal = batch.Donations.Sum(f => f.Fee);
            }


            // 3. Implement save of the batch here - down to the repo and get the object back with an id on it

            return batch;
        }
    }
}
