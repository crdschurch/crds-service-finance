using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services
{
    public class DonationService : IDonationService
    {
        private readonly IDonationRepository _mpDonationRepository;
        private readonly IDonationDistributionRepository _mpDonationDistributionRepository;
        private readonly IPledgeRepository _mpPledgeRepository;
        private readonly IMapper _mapper;
        private readonly IContactService _contactService;

        const int imInPledgeId = 1103;
        readonly DateTime imInObsessedStartDate = DateTime.Parse("3/18/2018");
        readonly DateTime imInObsessedEndDate = DateTime.Parse("12/31/2019");

        public DonationService(IDonationRepository mpDonationRepository, IDonationDistributionRepository mpDonationDistributionRepository, IPledgeRepository mpPledgeRepository, IMapper mapper,
            IContactService contactService)
        {
            _mpDonationRepository = mpDonationRepository;
            _mpDonationDistributionRepository = mpDonationDistributionRepository;
            _mpPledgeRepository = mpPledgeRepository;
            _mapper = mapper;
            _contactService = contactService;
        }

        public async Task<DonationDto> GetDonationByTransactionCode(string transactionCode)
        {
            var mpDonation = await _mpDonationRepository.GetDonationByTransactionCode(transactionCode);
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

        public async Task<DonationDto> Update(DonationDto donation)
        {
            var mpDonation = await _mpDonationRepository.Update(_mapper.Map<MpDonation>(donation));
            return _mapper.Map<DonationDto>(mpDonation);
        }

        public async Task<List<DonationDto>> Update(List<DonationDto> donations)
        {
            var mpDonations = await _mpDonationRepository.Update(_mapper.Map<List<MpDonation>>(donations));
            return _mapper.Map<List<DonationDto>>(mpDonations);
        }

        public async Task<MpDonor> CreateDonor(MpDonor donor)
        {
            return await _mpDonationRepository.CreateDonor(donor);
        }

        public async Task<MpDonorAccount> CreateDonorAccount(MpDonorAccount donor)
        {
            return await _mpDonationRepository.CreateDonorAccount(donor);
        }

        public void UpdateDonorAccount(JObject donorAccount)
        {
            _mpDonationRepository.UpdateDonorAccount(donorAccount);
        }

        public async Task<List<RecurringGiftDto>> GetRecurringGifts(int contactId)
        {
            var records = await _mpDonationRepository.GetRecurringGifts(contactId);
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

        public async Task<List<RecurringGiftDto>> GetRelatedContactRecurringGifts(int userContactId, int relatedContactId)
        {
            // check if household minor child
            var userContact = await _contactService.GetContact(userContactId);
            var householdMinorChildren = await _contactService.GetHouseholdMinorChildren(userContact.HouseholdId.Value);
            if (householdMinorChildren.Exists(householdChild => householdChild.ContactId == relatedContactId))
            {
                var mpRecurringGifts = await GetRecurringGifts(relatedContactId);
                return _mapper.Map<List<RecurringGiftDto>>(mpRecurringGifts);
            }

            // check if relatedContactId has an active co-giver contact relationship with userContactId
            var cogiverContactRelationship = await _contactService.GetCogiverContactRelationship(userContactId, relatedContactId);
            if (cogiverContactRelationship != null)
            {
                // relatedContactId is a cogiver contact relationship
                var mpRecurringGifts = await _mpDonationRepository.GetRecurringGiftsByContactIdAndDates(relatedContactId,
                    cogiverContactRelationship.StartDate,
                    cogiverContactRelationship.EndDate);
                return _mapper.Map<List<RecurringGiftDto>>(mpRecurringGifts);
            }
            throw new Exception($"Contact {userContactId} does not have access to view giving history for contact {relatedContactId}");
        }

        public async Task<List<PledgeDto>> GetPledges(int contactId)
        {
            var mpPledges = await CalculatePledges(contactId);
            return _mapper.Map<List<PledgeDto>>(mpPledges);
        }

        public async Task<List<PledgeDto>> GetRelatedContactPledge(int userContactId, int relatedContactId)
        {
            // check if household minor child
            var userContact = await _contactService.GetContact(userContactId);
            var householdMinorChildren = await _contactService.GetHouseholdMinorChildren(userContact.HouseholdId.Value);
            if (householdMinorChildren.Exists(householdChild => householdChild.ContactId == relatedContactId))
            {
                var mpPledges = await GetPledges(relatedContactId);
                return _mapper.Map<List<PledgeDto>>(mpPledges);
            }

            // check if relatedContactId has an active co-giver contact relationship with userContactId
            var cogiverContactRelationship = await _contactService.GetCogiverContactRelationship(userContactId, relatedContactId);
            if (cogiverContactRelationship != null)
            {
                var mpPledges = await GetPledges(relatedContactId);
                return _mapper.Map<List<PledgeDto>>(mpPledges);
            }
            throw new Exception($"Contact {userContactId} does not have access to view campaigns for contact {relatedContactId}");
        }

        public async Task<List<MpPledge>> CalculatePledges(int contactId)
        {
            var mpPledges = await _mpPledgeRepository.GetActiveAndCompleted(contactId);

            if (mpPledges.Any())
            {
                // get totals donations so far for this pledge
                var donationDistributions = await _mpDonationDistributionRepository.GetByPledges(mpPledges.Select(r => r.PledgeId).ToList());
                foreach (var mpPledge in mpPledges)
                {
                    var donationsForPledge = donationDistributions.Where(dd => dd.PledgeId == mpPledge.PledgeId).ToList();
                    mpPledge.PledgeDonationsTotal = donationsForPledge.Sum(dd => dd.Amount);
                    if (mpPledge.PledgeCampaignId == imInPledgeId)
                    {
                        SetImInPledgeInfo(mpPledge);
                    }
                }
            }

            return mpPledges;
        }

        public MpPledge SetImInPledgeInfo(MpPledge mpPledge) {
            if (mpPledge.FirstInstallmentDate >= imInObsessedStartDate)
            {
                mpPledge.CampaignName = "I'm In: Obsessed";
                mpPledge.CampaignStartDate = imInObsessedStartDate;
                mpPledge.CampaignEndDate = imInObsessedEndDate;
            }

            return mpPledge;
        }

        public async Task<List<DonationDetailDto>> GetDonations(string token)
        {
            var contactId = await _contactService.GetContactIdBySessionId(token);
            var records = await _mpDonationRepository.GetDonationHistoryByContactId(contactId);
            return _mapper.Map<List<DonationDetailDto>>(records);
        }

        public async Task<List<DonationDetailDto>> GetDonations(int contactId)
        {
            var records = await _mpDonationRepository.GetDonationHistoryByContactId(contactId);
            return _mapper.Map<List<DonationDetailDto>>(records);
        }

        public async Task<List<DonationDetailDto>> GetOtherGifts(int contactId)
        {
            var records = await _mpDonationRepository.GetOtherGiftsByContactId(contactId);
            return _mapper.Map<List<DonationDetailDto>>(records);
        }

        public async Task<List<DonationDetailDto>> GetRelatedContactOtherGifts(int userContactId, int relatedContactId)
        {
            // check if household minor child
            var userContact = await _contactService.GetContact(userContactId);
            var householdMinorChildren = await _contactService.GetHouseholdMinorChildren(userContact.HouseholdId.Value);
            if (householdMinorChildren.Exists(householdChild => householdChild.ContactId == relatedContactId))
            {
                var mpDonationHistory = await GetOtherGifts(relatedContactId);
                return _mapper.Map<List<DonationDetailDto>>(mpDonationHistory);
            }

            // check if relatedContactId has an active co-giver contact relationship with userContactId
            var cogiverContactRelationship = await _contactService.GetCogiverContactRelationship(userContactId, relatedContactId);
            if (cogiverContactRelationship != null)
            {
                // relatedContactId is a cogiver contact relationship
                var mpDonationHistory = await _mpDonationRepository.GetOtherGiftsForRelatedContact(relatedContactId,
                    cogiverContactRelationship.StartDate,
                    cogiverContactRelationship.EndDate);
                return _mapper.Map<List<DonationDetailDto>>(mpDonationHistory);
            }
            throw new Exception($"Contact {userContactId} does not have access to view giving history for contact {relatedContactId}");
        }

        public async Task<List<DonationDetailDto>> GetRelatedContactDonations(int userContactId, int relatedContactId)
        {
            // check if household minor child
            var userContact = await _contactService.GetContact(userContactId);
            var householdMinorChildren = await _contactService.GetHouseholdMinorChildren(userContact.HouseholdId.Value);
            if (householdMinorChildren.Exists(householdChild => householdChild.ContactId == relatedContactId))
            {
                var mpDonationHistory = await GetDonations(relatedContactId);
                return _mapper.Map<List<DonationDetailDto>>(mpDonationHistory);
            }

            // check if relatedContactId has an active co-giver contact relationship with userContactId
            var cogiverContactRelationship = await _contactService.GetCogiverContactRelationship(userContactId, relatedContactId);
            if (cogiverContactRelationship != null)
            {
                // relatedContactId is a cogiver contact relationship
                var mpDonationHistory = await _mpDonationRepository.GetDonationHistoryByContactId(relatedContactId,
                                                                           cogiverContactRelationship.StartDate,
                                                                           cogiverContactRelationship.EndDate);
                return _mapper.Map<List<DonationDetailDto>>(mpDonationHistory);
            }
            throw new Exception($"Contact {userContactId} does not have access to view giving history for contact {relatedContactId}");
        }
    }
}
