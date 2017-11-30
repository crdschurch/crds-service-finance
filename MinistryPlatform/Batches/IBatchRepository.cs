using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace MinistryPlatform.Batches
{
    public interface IBatchRepository
    {
        MpDonationBatch CreateDonationBatch(MpDonationBatch mpDonationBatch);
    }
}
