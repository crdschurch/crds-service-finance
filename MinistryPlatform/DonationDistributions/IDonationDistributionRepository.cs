using System.Collections.Generic;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IDonationDistributionRepository
    {
        List<MpDonationDistribution> GetByPledge(int pledgeId);
    }
}
