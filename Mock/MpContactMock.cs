using System.Collections.Generic;
using MinistryPlatform.Models;

namespace Mock
{
    public class MpContactMock
    {
        public static MpContact Create() =>
            new MpContact
            {
                ContactId = 22
            };

        public static List<MpContact> CreateList() =>
            new List<MpContact>
            {
                new MpContact
                {
                    ContactId = 22
                }
            };
    }
}
