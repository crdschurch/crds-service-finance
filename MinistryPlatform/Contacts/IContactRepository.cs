using System.Collections.Generic;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IContactRepository
    {
        MpContact MatchContact(string firstName, string lastName, string phone, string email);
    }
}
