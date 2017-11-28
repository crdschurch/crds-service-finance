using System.Collections.Generic;
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

        public static ContactDto CreateEmpty() => new ContactDto { };

    }
}
