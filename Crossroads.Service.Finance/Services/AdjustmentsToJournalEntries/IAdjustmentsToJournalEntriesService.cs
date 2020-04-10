using MinistryPlatform.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IAdjustmentsToJournalEntriesService
    {
        Task<List<MpJournalEntry>> Convert(List<MpDistributionAdjustment> mpDistributionAdjustments);
    }
}
