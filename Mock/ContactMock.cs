using System.Collections.Generic;
using Crossroads.Service.Finance.Models;

namespace Mock
{
    public class ContactMock
    {
        public static ContactDto Create() =>
            new ContactDto
            {
                ContactId = 22,
                HouseholdId = 456,
                Nickname = "Charles",
                LastName = "Norris"
            };

        public static List<ContactDto> CreateList() =>
            new List<ContactDto>
            {
                new ContactDto
                {
                    Nickname = "George",
                    LastName = "Grande",
                    ContactId = 22,
                    HouseholdId = 456
                },
                new ContactDto
                {
                    Nickname = "Bob",
                    LastName = "Williams",
                    ContactId = 22,
                    HouseholdId = 898
                }
            };
    }
}
