using MinistryPlatform.Models;
using System.Collections.Generic;

namespace MinistryPlatform.Interfaces
{
    public interface IPledgeRepository
    {
        List<MpPledge> GetActiveAndCompleted(int contactId);
    }
}
