using System.Collections.Generic;
using MinistryPlatform.Models;

namespace Mock
{
    public class MpContactRelationshipMock
    {
        public static List<MpContactRelationship> CreateList() =>
            new List<MpContactRelationship>
            {
                new MpContactRelationship
                {
                    RelatedContactId = 23
                },
                new MpContactRelationship
                {
                    RelatedContactId = 24
                }
            };
    }
}
