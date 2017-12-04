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
    public class DonationService : IDonationService
    {
        private readonly MinistryPlatform.Donations.IDonationRepository _donationRepository;
        private readonly IMapper _mapper;

        public DonationService(IDonationRepository donationRepository, IMapper mapper)
        {
            _donationRepository = donationRepository;
            _mapper = mapper;
        }

        public DonationDto GetDonationByTransactionCode(string transactionCode)
        {
            return (Mapper.Map<MpDonation, DonationDto>(_donationRepository.GetDonationByTransactionCode(transactionCode)));
        }

        // need to make sure to handle declined or refunded - do not change status
        public List<DonationDto> SetDonationStatus(List<DonationDto> donations, int batchId)
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

        public List<DonationDto> UpdateDonations(List<DonationDto> donations)
        {
            var mpDonations = _donationRepository.UpdateDonations(_mapper.Map<List<MpDonation>>(donations));
            return _mapper.Map<List<DonationDto>>(mpDonations);
        }
    }
}
