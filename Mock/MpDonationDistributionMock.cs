using System;
using System.Collections.Generic;
using System.Text;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;

namespace Mock
{
    public class MpDonationDistributionMock
    {
        public static List<MpDonationDistribution> CreateList(int pledgeId1 = 12, int pledgeId2 = 4545) =>
            new List<MpDonationDistribution>
            {
                new MpDonationDistribution
                {
                    Amount = 23m,
                    PledgeId = pledgeId1,
                    DonationId = 12
                },
                new MpDonationDistribution
                {
                    Amount = 62.10m,
                    PledgeId = pledgeId2,
                    DonationId = 14
                },
                new MpDonationDistribution
                {
                    Amount = 87.12m,
                    PledgeId = pledgeId1,
                    DonationId = 188
                },
            };
    }
}
