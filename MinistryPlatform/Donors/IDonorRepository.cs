using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Models;

namespace MinistryPlatform.Donors
{
    public interface IDonorRepository
    {
        Task<MpDonor> CreateDonor(MpDonor donor);
        Task<int?> GetDonorIdByProcessorId(string processorId);
        Task<MpDonor> GetDonorByDonorId(int donorId);
    }
}
