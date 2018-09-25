using System.Collections.Generic;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MinistryPlatform.Interfaces
{
    public interface IRecurringGiftRepository
    {
        MpRecurringGift CreateRecurringGift(MpRecurringGift mpRecurringGift);
        void UpdateRecurringGift(JObject mpRecurringGift);
        MpRecurringGift FindRecurringGiftBySubscriptionId(string subscriptionId);
        List<MpRecurringGift> FindRecurringGiftsByDonorId(int donorId);
        List<MpRecurringGift> FindRecurringGiftsBySubscriptionIds(List<string> subscriptionIds);
    }
}
