using Crossroads.Service.Template.Models;

namespace Crossroads.Service.Template.Services.Interfaces
{
    public interface IContactService
    {
        ContactDto GetContact(int contactId);
    }
}
