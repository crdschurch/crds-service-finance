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

        public static List<MpPledge> CreateList(int pledge1 = 22, int pledge2 = 266, int pledge3 = 45) =>
            new List<MpPledge>
            {
                new MpPledge
                {
                    PledgeId = pledge1,
                    PledgeTotal = 1200
                },
                new MpPledge
                {
                    PledgeId = pledge2,
                    PledgeTotal = 10000
                },
                new MpPledge
                {
                    PledgeId = pledge3,
                    PledgeTotal = 6400
                },
            };

        public static MpDonation CreateEmpty() => new MpDonation { };
    }
}
