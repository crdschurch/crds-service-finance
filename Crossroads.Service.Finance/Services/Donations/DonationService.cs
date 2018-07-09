using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crossroads.Service.Finance.Services
{
    public class DonationService : IDonationService
    {
        private readonly IDonationRepository _mpDonationRepository;
        private readonly IDonationDistributionRepository _mpDonationDistributionRepository;
        private readonly IPledgeRepository _mpPledgeRepository;
        private readonly IMapper _mapper;

        public DonationService(IDonationRepository mpDonationRepository, IDonationDistributionRepository mpDonationDistributionRepository, IPledgeRepository mpPledgeRepository, IMapper mapper)
        {
            _mpDonationRepository = mpDonationRepository;
            _mpDonationDistributionRepository = mpDonationDistributionRepository;
            _mpPledgeRepository = mpPledgeRepository;
            _mapper = mapper;
        }

        public DonationDto GetDonationByTransactionCode(string transactionCode)
        {
            var mpDonation = _mpDonationRepository.GetDonationByTransactionCode(transactionCode);
            if (mpDonation == null) return null;
            var donationDto = _mapper.Map<DonationDto>(mpDonation);
            return donationDto;
        }

        // need to make sure to handle declined or refunded - do not change status
        public List<DonationDto> SetDonationStatus(List<DonationDto> donations, int batchId)
        {
            var updatedDonations = donations;

            foreach (var updatedDonation in updatedDonations)
            {
                if (updatedDonation.DonationStatusId != DonationStatus.Declined.GetHashCode() &&
                    updatedDonation.DonationStatusId != DonationStatus.Refunded.GetHashCode())
                {
                    updatedDonation.DonationStatusId = DonationStatus.Deposited.GetHashCode();
                }

                updatedDonation.BatchId = batchId;
            }

            return updatedDonations;
        }

        public DonationDto Update(DonationDto donation)
        {
            var mpDonation = _mpDonationRepository.Update(_mapper.Map<MpDonation>(donation));
            return _mapper.Map<DonationDto>(mpDonation);
        }

        public List<DonationDto> Update(List<DonationDto> donations)
        {
            var mpDonations = _mpDonationRepository.Update(_mapper.Map<List<MpDonation>>(donations));
            return _mapper.Map<List<DonationDto>>(mpDonations);
        }

        public MpDonor CreateDonor(MpDonor donor)
        {
            return _mpDonationRepository.CreateDonor(donor);
        }

        public MpDonorAccount CreateDonorAccount(MpDonorAccount donor)
        {
            return _mpDonationRepository.CreateDonorAccount(donor);
        }

        public void UpdateDonorAccount(JObject donorAccount)
        {
            _mpDonationRepository.UpdateDonorAccount(donorAccount);
        }
        
        public List<RecurringGiftDto> GetRecurringGifts(string token)
        {
            //TODO: Remove hard coding and get actual contact id from token
            int contactId = 7516930;
            var records = _mpDonationRepository.GetRecurringGifts(contactId);
            return _mapper.Map<List<RecurringGiftDto>>(records);
        }

        public List<PledgeDto> GetPledges(string token)
        {
            //TODO: Remove hard coding and get actual contact id from token
            int contactId = 7647737;
            var mpPledges = CalculatePledges("token");
            return _mapper.Map<List<PledgeDto>>(mpPledges);
        }

        public List<MpPledge> CalculatePledges(string token)
        {
            //TODO: Remove hard coding and get actual contact id from token
            int contactId = 7647737;
            var mpPledges = _mpPledgeRepository.GetActiveAndCompleted(contactId);
            // get totals donations so far for this pledge
            var donationDistributions = _mpDonationDistributionRepository.GetByPledges(mpPledges.Select(r => r.PledgeId).ToList());
            foreach (var mpPledge in mpPledges)
            {
                var donationsForPledge = donationDistributions.Where(dd => dd.PledgeId == mpPledge.PledgeId).ToList();
                mpPledge.PledgeDonationsTotal = donationsForPledge.Sum(dd => dd.Amount);
            }
            return mpPledges;
        }

        public List<DonationDto> GetDonations(string token)
        {
            //TODO: Remove hard coding and get actual contact id from token
            int contactId = 7516930;
            var records = _mpDonationRepository.GetDonations(contactId);
            return _mapper.Map<List<DonationDto>>(records);
        }

        public List<DonationHistoryDto> GetDonationHistoryByContactId(int contactId)
        {
            var donationHistoryDtos = _mpDonationRepository.GetDonationHistoryByContactId(contactId);
            return _mapper.Map<List<DonationHistoryDto>>(donationHistoryDtos);
        }
    }
}
