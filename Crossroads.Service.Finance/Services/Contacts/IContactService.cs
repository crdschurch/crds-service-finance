using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IContactService
    {
        int GetContactIdBySessionId(string sessionId);
        ContactDto GetContact(int contactId);
        ContactDto GetBySessionId(string sessionId);
        List<ContactDto> GetCogiversByContactId(int contactId);
        ContactRelationship GetCogiverContactRelationship(int contactId, int relatedContactId);
        List<ContactDto> GetHouseholdMinorChildren(int householdId);
        List<ContactDto> GetDonorRelatedContacts(string token);
        ContactAddressDto GetContactAddressByContactId(int contactId);
    }
}
