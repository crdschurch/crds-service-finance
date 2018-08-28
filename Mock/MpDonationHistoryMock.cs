using System.Collections.Generic;
using MinistryPlatform.Models;

namespace Mock
{
    public class MpDonationDetailMock
    {
        public static MpDonationDetail Create() =>
            new MpDonationDetail
            {
                DonationId = 22
            };

        public static List<MpDonationDetail> CreateList() =>
            new List<MpDonationDetail>
            {
                new MpDonationDetail
                {
                    DonationId = 23
                },
                new MpDonationDetail
                {
                    DonationId = 24
                },
                new MpDonationDetail
                {
                    DonationId = 25
                },
            };

        public static List<MpDonationDetail> CreateEmpty() => new List<MpDonationDetail> { };
    }
}
