using System.Collections.Generic;
using System.Threading.Tasks;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Interfaces
{
    public interface IRecurringGiftRepository
    {
        Task<MpRecurringGift> CreateRecurringGift(MpRecurringGift mpRecurringGift);
        void UpdateRecurringGift(JObject mpRecurringGift);
        Task<MpRecurringGift> FindRecurringGiftBySubscriptionId(string subscriptionId);
        Task<List<MpRecurringGift>> FindRecurringGiftsByDonorId(int donorId);
        Task<List<MpRecurringGift>> FindRecurringGiftsBySubscriptionIds(List<string> subscriptionIds);
    }
}
