using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Services.Interfaces
{
    public interface IContactService
    {
        ContactDto GetContact(int contactId);
    }
}
