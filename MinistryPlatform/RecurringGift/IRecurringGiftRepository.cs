using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Interfaces
{
    public interface IRecurringGiftRepository
    {
        MpRecurringGift CreateRecurringGift(MpRecurringGift mpRecurringGift);
        void UpdateRecurringGift(JObject mpRecurringGift);
        MpRecurringGift FindRecurringGiftBySubscriptionId(string subscriptionId);
    }
}
