using System.Collections.Generic;
using System.Threading.Tasks;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IDonationDistributionRepository
    {
        Task<List<MpDonationDistribution>> GetByPledges(List<int> pledgeIds);
        Task<List<MpDonationDistribution>> GetByDonationId(int donationId);
        Task<List<MpDonationDistribution>> UpdateDonationDistributions(List<MpDonationDistribution> mpDonationDistributions);
    }
}
