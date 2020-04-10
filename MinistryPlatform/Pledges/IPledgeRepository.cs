using MinistryPlatform.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MinistryPlatform.Interfaces
{
    public interface IPledgeRepository
    {
        Task<List<MpPledge>> GetActiveAndCompleted(int contactId);
    }
}
