using System.Collections.Generic;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IContactRepository
    {
        MpDonor MatchContact(string firstName, string lastName, string phone, string email);
        MpHousehold GetHousehold(int householdId);
        int GetBySessionId(string sessionId);
        MpContact GetContact(int contactId);
        List<MpContactRelationship> GetContactRelationships(int contactId, int contactRelationshipId);
        MpContactRelationship GetContactRelationship(int contactId, int relatedContactId, int contactRelationshipId);
        List<MpContact> GetHouseholdMinorChildren(int householdId);
    }
}
