using Crossroads.Service.Finance.Models;
using MinistryPlatform.Interfaces;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using System.Collections.Generic;
using MinistryPlatform.Models;
using System.Linq;
using System;
using System.Threading.Tasks;
using MinistryPlatform.Users;

namespace Crossroads.Service.Finance.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IUserRepository _userRepository;
        IMapper _mapper;

        private const int cogiverRelationshipId = 42;

        public ContactService(IContactRepository contactRepository, IUserRepository userRepository, IMapper mapper)
        {
            _contactRepository = contactRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<int> GetContactIdBySessionId(string sessionId)
        {
            var contactId = await _contactRepository.GetBySessionId(sessionId);
            return contactId;
        }

        public async Task<ContactDto> GetContact(int contactId)
        {
            var mpContact = await _contactRepository.GetContact(contactId);
            if (mpContact != null)
            {
                return _mapper.Map<ContactDto>(mpContact);
            }
            return null;
        }

        public async Task<ContactDto> GetBySessionId(string sessionId)
        {
            var contactId = await _contactRepository.GetBySessionId(sessionId);
            var mpContact = await _contactRepository.GetContact(contactId);
            return _mapper.Map<ContactDto>(mpContact);
        }

        public Task<ContactDto> GetContactBySessionId(string sessionId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ContactDto>> GetCogiversByContactId(int contactId)
        {
            // TODO: If the performance needs to be improved, consider moving to a proc to
            // reduce number of service calls
            var contactRelationships = await _contactRepository.GetActiveContactRelationships(contactId, cogiverRelationshipId);

            var cogivers = new List<MpContact>();
            foreach (MpContactRelationship relatedContact in contactRelationships)
            {
                var contact = await _contactRepository.GetContact(relatedContact.RelatedContactId);
                cogivers.Add(contact);
            }

            return _mapper.Map<List<ContactDto>>(cogivers);
        }

        public async Task<ContactRelationship> GetCogiverContactRelationship(int contactId, int relatedContactId)
        {
            var contactRelationship = await _contactRepository.GetActiveContactRelationship(contactId, relatedContactId, cogiverRelationshipId);
            return _mapper.Map<ContactRelationship>(contactRelationship);
        }

        public async Task<List<ContactDto>> GetHouseholdMinorChildren(int householdId)
        {
            var householdMinorChildren = await _contactRepository.GetHouseholdMinorChildren(householdId);
            return _mapper.Map<List<ContactDto>>(householdMinorChildren);
        }

        public async Task<List<ContactDto>> GetDonorRelatedContacts(int userContactId)
        {
            var userContact = await GetContact(userContactId);
            var cogivers = await GetCogiversByContactId(userContactId);
            var userDonationVisibleContacts = new List<ContactDto>();
            var householdMinorChildren = await GetHouseholdMinorChildren(userContact.HouseholdId.Value);
            userDonationVisibleContacts.AddRange(cogivers);
            userDonationVisibleContacts.AddRange(householdMinorChildren);
            userDonationVisibleContacts = userDonationVisibleContacts.OrderBy(c => c.Nickname).ThenBy(c => c.FirstName).ThenBy(c => c.LastName).ToList();
            userDonationVisibleContacts.Insert(0, userContact);
            return userDonationVisibleContacts;
        }

        public async Task<ContactAddressDto> GetContactAddressByContactId(int contactId)
        {
            var mpContactAddress = await _contactRepository.GetContactAddressByContactId(contactId);
            return _mapper.Map<ContactAddressDto>(mpContactAddress);
        }

        public async Task<int> GetContactIdByEmailAddress(string emailAddress)
        {
            var contactId = await _userRepository.GetUserByEmailAddress(emailAddress);
            return contactId;
        }
    }
}