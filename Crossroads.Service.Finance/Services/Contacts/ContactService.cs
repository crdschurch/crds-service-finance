using Crossroads.Service.Finance.Models;
using MinistryPlatform.Interfaces;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using System.Collections.Generic;
using MinistryPlatform.Models;

namespace Crossroads.Service.Finance.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        IMapper _mapper;

        private const int cogiverRelationshipId = 42;

        public ContactService(IContactRepository contactRepository, IMapper mapper)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
        }

        public int GetContactIdBySessionId(string sessionId)
        {
            var contactId = _contactRepository.GetBySessionId(sessionId);
            return contactId;
        }

        public ContactDto GetContact(int contactId)
        {
            var mpContact = _contactRepository.GetContact(contactId);
            if (mpContact != null)
            {
                return _mapper.Map<ContactDto>(mpContact);
            }
            return null;
        }

        public ContactDto GetBySessionId(string sessionId)
        {
            var contactId = _contactRepository.GetBySessionId(sessionId);
            var mpContact = _contactRepository.GetContact(contactId);
            return _mapper.Map<ContactDto>(mpContact);
        }

        public List<ContactDto> GetCogiversByContactId(int contactId)
        {
            // TODO: If the performance needs to be improved, consider moving to a proc to
            // reduce number of service calls
            var contactRelationships = _contactRepository.GetContactRelationships(contactId, cogiverRelationshipId);

            var cogivers = new List<MpContact>();
            foreach (MpContactRelationship relatedContact in contactRelationships)
            {
                var contact = _contactRepository.GetContact(relatedContact.RelatedContactId);
                cogivers.Add(contact);
            }

            return _mapper.Map <List<ContactDto>>(cogivers);
        }
    }
}
