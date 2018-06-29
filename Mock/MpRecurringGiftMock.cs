using MinistryPlatform.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mock
{
    public class MpRecurringGiftMock
    {

        public static List<MpRecurringGift> CreateList(int contactId) =>
            new List<MpRecurringGift>
            {
                new MpRecurringGift
                {
                    RecurringGiftId = 1,
                    ContactId = contactId,
                    DonorId = 123,
                    DonorAccountId = 123,
                    FrequencyId = 2,
                    DayOfMonth = 5,
                    DayOfWeek = 3,
                    Amount = 25,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today,
                    ProgramId = 10,
                    CongregationId = 100,
                    SubscriptionId = "100",
                    ConsecutiveFailureCount = 2,
                    SourceUrl = "http://localhost",
                    PredefinedAmount = 25,
                    VendorDetailUrl = "http://localhost"
                }
            };

        public static List<MpRecurringGift> CreateEmptyList()
        {
            return new List<MpRecurringGift>();
        }
    }
}