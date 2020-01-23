using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IContactService
    {
        Task<int> GetContactIdBySessionId(string sessionId);
        Task<ContactDto> GetContact(int contactId);
        Task<List<ContactDto>> GetCogiversByContactId(int contactId);
        Task<ContactRelationship> GetCogiverContactRelationship(int contactId, int relatedContactId);
        Task<List<ContactDto>> GetHouseholdMinorChildren(int householdId);
        Task<List<ContactDto>> GetDonorRelatedContacts(int contactId);
        Task<ContactAddressDto> GetContactAddressByContactId(int contactId);
        Task<int> GetContactIdByEmailAddress(string emailAddress);
    }
}
