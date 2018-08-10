using Crossroads.Service.Finance.Models;

namespace Mock
{
    public class ContactMock
    {
        public static ContactDto Create() =>
            new ContactDto
            {
                ContactId = 22
            };
    }
}
