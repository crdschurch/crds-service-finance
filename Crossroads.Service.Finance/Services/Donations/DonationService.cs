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
        private readonly IDonationDistributionRepository _mpDonationDistributionRepository;
        private readonly IPledgeRepository _mpPledgeRepository;
        private readonly IMapper _mapper;
        private readonly IContactService _contactService;

        public DonationService(IDonationRepository mpDonationRepository, IDonationDistributionRepository mpDonationDistributionRepository, IPledgeRepository mpPledgeRepository, IMapper mapper,
            IContactService contactService)
        {
            _mpDonationRepository = mpDonationRepository;
            _mpDonationDistributionRepository = mpDonationDistributionRepository;
            _mpPledgeRepository = mpPledgeRepository;
            _mapper = mapper;
            _contactService = contactService;
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
            var contactId = _contactService.GetContactIdBySessionId(token);
            var records = _mpDonationRepository.GetRecurringGifts(contactId);
            var dtos = _mapper.Map<List<RecurringGiftDto>>(records);

            // mark stripe gifts as not having a valid subscription status
            foreach (var dto in dtos)
            {
                if (dto.SubscriptionId.Take(4).ToString() == "sub_")
                {
                    dto.RecurringGiftStatusId = 0;
                }
            }

            return dtos;
        }

        public List<PledgeDto> GetPledges(int contactId)
        {
            var mpPledges = CalculatePledges(contactId);
            return _mapper.Map<List<PledgeDto>>(mpPledges);
        }

        public List<PledgeDto> GetRelatedContactPledge(int userContactId, int relatedContactId)
        {
            // check if household minor child
            var userContact = _contactService.GetContact(userContactId);
            var householdMinorChildren = _contactService.GetHouseholdMinorChildren(userContact.HouseholdId.Value);
            if (householdMinorChildren.Exists(householdChild => householdChild.ContactId == relatedContactId))
            {
                var mpPledges = GetPledges(relatedContactId);
                return _mapper.Map<List<PledgeDto>>(mpPledges);
            }

            // check if relatedContactId has an active co-giver contact relationship with userContactId
            var cogiverContactRelationship = _contactService.GetCogiverContactRelationship(userContactId, relatedContactId);
            if (cogiverContactRelationship != null)
            {
                var mpPledges = GetPledges(relatedContactId);
                return _mapper.Map<List<PledgeDto>>(mpPledges);
            }
            throw new Exception($"Contact {userContactId} does not have access to view campaigns for contact {relatedContactId}");
        }

        public List<MpPledge> CalculatePledges(int contactId)
        {
            var mpPledges = _mpPledgeRepository.GetActiveAndCompleted(contactId);

            if (mpPledges.Any())
            {
                // get totals donations so far for this pledge
                var donationDistributions = _mpDonationDistributionRepository.GetByPledges(mpPledges.Select(r => r.PledgeId).ToList());
                foreach (var mpPledge in mpPledges)
                {
                    var donationsForPledge = donationDistributions.Where(dd => dd.PledgeId == mpPledge.PledgeId).ToList();
                    mpPledge.PledgeDonationsTotal = donationsForPledge.Sum(dd => dd.Amount);
                }
            }

            return mpPledges;
        }

        public List<DonationDetailDto> GetDonations(string token)
        {
            var contactId = _contactService.GetContactIdBySessionId(token);
            var records = _mpDonationRepository.GetDonationHistoryByContactId(contactId);
            return _mapper.Map<List<DonationDetailDto>>(records);
        }

        public List<DonationDetailDto> GetDonations(int contactId)
        {
            var records = _mpDonationRepository.GetDonationHistoryByContactId(contactId);
            return _mapper.Map<List<DonationDetailDto>>(records);
        }

        public List<DonationDetailDto> GetRelatedContactDonations(int userContactId, int relatedContactId)
        {
            // check if household minor child
            var userContact = _contactService.GetContact(userContactId);
            var householdMinorChildren = _contactService.GetHouseholdMinorChildren(userContact.HouseholdId.Value);
            if (householdMinorChildren.Exists(householdChild => householdChild.ContactId == relatedContactId))
            {
                var mpDonationHistory = GetDonations(relatedContactId);
                return _mapper.Map<List<DonationDetailDto>>(mpDonationHistory);
            }

            // check if relatedContactId has an active co-giver contact relationship with userContactId
            var cogiverContactRelationship = _contactService.GetCogiverContactRelationship(userContactId, relatedContactId);
            if (cogiverContactRelationship != null)
            {
                // relatedContactId is a cogiver contact relationship
                var mpDonationHistory = _mpDonationRepository.GetDonationHistoryByContactId(relatedContactId,
                                                                           cogiverContactRelationship.StartDate,
                                                                           cogiverContactRelationship.EndDate);
                return _mapper.Map<List<DonationDetailDto>>(mpDonationHistory);
            }
            throw new Exception($"Contact {userContactId} does not have access to view giving history for contact {relatedContactId}");
        }
    }
}
