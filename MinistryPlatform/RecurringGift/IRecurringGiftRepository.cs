using System.Collections.Generic;
using System.Threading.Tasks;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Interfaces
{
    public interface IRecurringGiftRepository
    {
        MpRecurringGift CreateRecurringGift(MpRecurringGift mpRecurringGift);
        void UpdateRecurringGift(JObject mpRecurringGift);
        Task<MpRecurringGift> FindRecurringGiftBySubscriptionId(string subscriptionId);
        List<MpRecurringGift> FindRecurringGiftsByDonorId(int donorId);
        List<MpRecurringGift> FindRecurringGiftsBySubscriptionIds(List<string> subscriptionIds);
    }
}
