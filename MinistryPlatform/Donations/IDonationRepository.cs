using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace MinistryPlatform.Donations
{
    public interface IDonationRepository
    {
        MpDonation GetDonationByTransactionCode(string transactionCode); // theoretically on settlement as transactionid
        List<MpDonation> UpdateDonations(List<MpDonation> donations);
    }
}
