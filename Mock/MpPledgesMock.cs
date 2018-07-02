using System;
using System.Collections.Generic;
using System.Text;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;

namespace Mock
{
    public class MpPledgeMock
    {
        public static MpPledge Create() =>
        new MpPledge
            {
                PledgeId = 22,
                PledgeTotal = 1200
            };

        public static List<MpPledge> CreateList() =>
            new List<MpPledge>
            {
                new MpPledge
                {
                    PledgeId = 22,
                    PledgeTotal = 1200
                },
                new MpPledge
                {
                    PledgeId = 266,
                    PledgeTotal = 10000
                },
                new MpPledge
                {
                    PledgeId = 45,
                    PledgeTotal = 6400
                },
            };

        public static MpDonation CreateEmpty() => new MpDonation { };
    }
}
