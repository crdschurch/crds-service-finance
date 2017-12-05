using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IBatchRepository
    {
        MpDonationBatch CreateDonationBatch(MpDonationBatch mpDonationBatch);
        void UpdateDonationBatch(MpDonationBatch mpDonationBatch);
    }
}
