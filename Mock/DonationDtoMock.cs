using System.Collections.Generic;
using Crossroads.Service.Finance.Models;

namespace Mock
{
    public class DonationDtoMock
    {
        public static DonationDto CreatePending(string transactionCode) =>
            new DonationDto
            {
                TransactionCode = transactionCode,
                DonationAmt = "14",
                DonationStatusId = 1 // pending
            };

        public static DonationDto CreateDeclined(string transactionCode) =>
            new DonationDto
            {
                TransactionCode = transactionCode,
                DonationAmt = "14",
                DonationStatusId = 3 // declined
                    };

        public static DonationDto CreateSucceeded(string transactionCode) =>
            new DonationDto
            {
                TransactionCode = transactionCode,
                DonationAmt = "14",
                DonationStatusId = 4 // succeeded
                    };

        public static DonationDto CreateEmpty() => new DonationDto { };
    }
}
