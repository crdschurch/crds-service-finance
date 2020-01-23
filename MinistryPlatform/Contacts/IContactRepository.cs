using System.Collections.Generic;
using System.Threading.Tasks;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IContactRepository
    {
        Task<MpDonor> MatchContact(string firstName, string lastName, string phone, string email);
        Task<MpHousehold> GetHousehold(int householdId);
        Task<int> GetBySessionId(string sessionId);
        Task<MpContact> GetContact(int contactId);
        Task<List<MpContactRelationship>> GetActiveContactRelationships(int contactId, int contactRelationshipId);
        Task<MpContactRelationship> GetActiveContactRelationship(int contactId, int relatedContactId, int contactRelationshipId);
        Task<List<MpContact>> GetHouseholdMinorChildren(int householdId);
        Task<MpContactAddress> GetContactAddressByContactId(int contactId);
    }
}
