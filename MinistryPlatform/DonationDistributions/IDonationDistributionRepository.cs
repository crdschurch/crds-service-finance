using System.Collections.Generic;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IDonationDistributionRepository
    {
        List<MpDonationDistribution> GetByPledges(List<int> pledgeIds);
        List<MpDonationDistribution> GetByDonationId(int donationId);
        List<MpDonationDistribution> UpdateDonationDistributions(List<MpDonationDistribution> mpDonationDistributions);
    }
}
