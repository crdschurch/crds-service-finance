using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IBatchRepository
    {
        Task<MpDonationBatch> CreateDonationBatch(MpDonationBatch mpDonationBatch);
        void UpdateDonationBatch(MpDonationBatch mpDonationBatch);
    }
}
