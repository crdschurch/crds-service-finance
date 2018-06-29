﻿using AutoMapper;
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
        private readonly IPledgeRepository _mpPledgeRepository;
        private readonly IMapper _mapper;

        public DonationService(IDonationRepository mpDonationRepository, IPledgeRepository mpPledgeRepository, IMapper mapper)
        {
            _mpDonationRepository = mpDonationRepository;
            _mpPledgeRepository = mpPledgeRepository;
            _mapper = mapper;
        }

        public DonationDto GetDonationByTransactionCode(string transactionCode)
        {
            var mpDonation = _mpDonationRepository.GetDonationByTransactionCode(transactionCode);
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
            int contactId = 7516930;
            var records = _mpDonationRepository.GetRecurringGifts(contactId);
            return records.Select(Mapper.Map<MpRecurringGift, RecurringGiftDto>).ToList();
        }

        public IList<PledgeDto> GetPledges(string token)
        {
            int contactId = 12;
            var mpPledges = _mpPledgeRepository.GetActiveAndCompleted(contactId);
            return _mapper.Map<List<PledgeDto>>(mpPledges);
        }

        public List<DonationDto> GetDonations(string token)
        {
            int contactId = 7516930;
            var records = _mpDonationRepository.GetDonations(contactId);
            return records.Select(Mapper.Map<MpDonation, DonationDto>).ToList();
        }

    }
}
