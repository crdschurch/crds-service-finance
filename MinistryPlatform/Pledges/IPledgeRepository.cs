using System.Collections.Generic;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IPledgeRepository
    {
        IList<MpPledge> GetActiveAndCompleted(int contactId);
    }
}
