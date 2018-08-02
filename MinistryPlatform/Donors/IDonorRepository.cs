using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace MinistryPlatform.Donors
{
    public interface IDonorRepository
    {
        int? GetDonorIdByProcessorId(string processorId);
        MpDonor GetDonorByDonorId(int donorId);
    }
}
