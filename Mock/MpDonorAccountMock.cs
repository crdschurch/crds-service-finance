using System;
using System.Collections.Generic;
using System.Text;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;

namespace Mock
{
    public class MpDonorAccountMock
    {
        public static MpDonorAccount Create() =>
            new MpDonorAccount
            {
                DonorAccountId = 777
            };


        public static List<MpDonorAccount> CreateEmpty() => new List<MpDonorAccount> { };
    }
}
