using System;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IContactRepository
    {
        MpContact GetContact(int contactId);
    }
}
