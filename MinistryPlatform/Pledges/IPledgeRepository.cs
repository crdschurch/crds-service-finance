using System.Collections.Generic;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IPledgeRepository
    {
        List<MpPledge> GetActiveAndCompleted(int contactId);
    }
}
