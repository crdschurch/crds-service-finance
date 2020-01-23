using System;
using System.Collections.Generic;
using System.Text;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;

namespace Mock
{
    public class MpDonationsMock
    {
        public static MpDonation Create() =>
            new MpDonation
            {
                DonationId = 22,
                TransactionCode = "0abc"
            };

        public static List<MpDonation> CreateList() =>
            new List<MpDonation>
            {
                new MpDonation
                {
                    DonationId = 23,
                    TransactionCode = "PP-1a"
                },
                new MpDonation
                {
                    DonationId = 24,
                    TransactionCode = "PP-2b"
                },
                new MpDonation
                {
                    DonationId = 25,
                    TransactionCode = "PP-3c"
                },
            };

        public static List<MpDonation> CreateListTask() =>
            new List<MpDonation>
            {
                new MpDonation
                {
                    DonationId = 23,
                    TransactionCode = "1a"
                },
                new MpDonation
                {
                    DonationId = 24,
                    TransactionCode = "2b"
                },
                new MpDonation
                {
                    DonationId = 25,
                    TransactionCode = "3c"
                },
            };

        public static List<MpDonation> CreateEmpty() => new List<MpDonation> { };
    }
}
