using System;
namespace MinistryPlatform.Models
{
    public class MpRecurringGiftDays
    {
        //public MpRecurringGiftDays()
        //{
        //}

        public static int GetMpRecurringGiftDay(DateTime dateTime)
        {
  
            switch (dateTime.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 7;
                case DayOfWeek.Monday:
                    return 1;
                case DayOfWeek.Tuesday:
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                default:
                    return 0;
            }
        }
    }
}
