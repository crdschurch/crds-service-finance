using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IAdjustmentsToJournalEntriesService
    {
        List<MpJournalEntry> Convert(List<MpDistributionAdjustment> mpDistributionAdjustments);
    }
}
