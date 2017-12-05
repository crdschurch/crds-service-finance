using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IContactService
    {
        ContactDto GetContact(int contactId);
    }
}
