using MinistryPlatform.Models;
using System.Collections.Generic;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IAdjustmentsToJournalEntriesService
    {
        List<MpJournalEntry> Convert(List<MpDistributionAdjustment> mpDistributionAdjustments);
    }
}
