using System;
using MinistryPlatform.Models;
using System.Collections.Generic;

namespace Mock
{
  public class MpContactMock
  {

        public static List<MpContact> CreateList(int contactId, int householdId) =>
            new List<MpContact>
            {
                new MpContact
                {
                    ContactId = contactId,
                    HouseholdId = householdId
                }
            };

        public static List<MpContact> CreateEmptyList()
        {
            return new List<MpContact>();
        }
    }
}
