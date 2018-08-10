using System.Collections.Generic;
using MinistryPlatform.Models;

namespace Mock
{
    public class MpDonationHistoryMock
    {
        public static MpDonationHistory Create() =>
            new MpDonationHistory
            {
                DonationId = 22
            };

        public static List<MpDonationHistory> CreateList() =>
            new List<MpDonationHistory>
            {
                new MpDonationHistory
                {
                    DonationId = 23
                },
                new MpDonationHistory
                {
                    DonationId = 24
                },
                new MpDonationHistory
                {
                    DonationId = 25
                },
            };

        public static List<MpDonationHistory> CreateEmpty() => new List<MpDonationHistory> { };
    }
}
