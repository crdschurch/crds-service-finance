using System.Collections.Generic;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IContactRepository
    {
        MpDonor MatchContact(string firstName, string lastName, string phone, string email);
        MpHousehold GetHousehold(int householdId);
        void UpdateProcessor(int donorId, string processorId);
        MpDonor FindDonorByProcessorId(string processorId);
        int GetBySessionId(string sessionId);
        MpContact GetContact(int contactId);
    }
}
