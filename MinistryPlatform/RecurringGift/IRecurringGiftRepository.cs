using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IRecurringGiftRepository
    {
        MpRecurringGift CreateRecurringGift(MpRecurringGift mpRecurringGift);
    }
}
