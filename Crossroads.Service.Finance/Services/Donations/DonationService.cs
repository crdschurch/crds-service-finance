using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Donations;
using MinistryPlatform.Models;

namespace Crossroads.Service.Finance.Services.Donations
{
    public class DonationService
    {
        private readonly MinistryPlatform.Donations.IDonationRepository _donationRepository;

        public DonationService(IDonationRepository donationRepository)
        {
            _donationRepository = donationRepository;
        }

        public DepositDto GetDepositByProcessorTransferId(string key)
        {
            return (Mapper.Map<MpDeposit, DepositDto>(_donationRepository.GetDepositByProcessorTransferId(key)));
        }

        public DonationDto GetDonationByTransactionCode(string transactionCode)
        {
            return (Mapper.Map<MpDonation, DonationDto>(_donationRepository.GetDonationByTransactionCode(transactionCode)));
        }

        // need to make sure to handle declined or refunded - do not change status
        public List<DonationDto> UpdateDonationStatus(List<DonationDto> donations, int batchId)
        {
            var updatedDonations = donations;

            foreach (var updatedDonation in updatedDonations)
            {
                // TODO: Verify that this is an or condition, not an and condition
                if (updatedDonation.Status != DonationStatus.Declined ||
                    updatedDonation.Status != DonationStatus.Refunded)
                {
                    updatedDonation.Status = DonationStatus.Deposited;
                }

                updatedDonation.BatchId = batchId;
            }

            return updatedDonations;
        }

        public void SaveDonations(List<DonationDto> donations)
        {
            // TODO: Save this down to the repo layer
        }
    }
}
