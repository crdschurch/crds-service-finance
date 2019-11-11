using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace Mock
{
    public class MpDistributionAdjustmentMock
    {
        public static MpDistributionAdjustment Create() =>
            new MpDistributionAdjustment
            {
                
            };

        public static List<MpDistributionAdjustment> CreateList() =>
            new List<MpDistributionAdjustment>
            {
                new MpDistributionAdjustment
                {
                    GLAccountNumber = "40001-325-02",
                    DonationDate = new DateTime(2019, 08, 01),
                    Amount = 1
                }
            };
    }
}
