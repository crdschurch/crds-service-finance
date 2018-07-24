using Crossroads.Service.Finance.Models;
using MinistryPlatform.Interfaces;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using System.Collections.Generic;

namespace Crossroads.Service.Finance.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        IMapper _mapper;

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
            var cogivers = _contactRepository.GetCogivers(contactId);
            return _mapper.Map <List<ContactDto>>(cogivers);
        }
    }
}
