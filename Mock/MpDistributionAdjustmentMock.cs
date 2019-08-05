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
                    GLAccountNumber = "test_gl_account",
                    DonationDate = new DateTime(2019, 08, 01),
                    Amount = 1
                }
            };
    }
}
