﻿using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace MinistryPlatform.Donations
{
    public interface IDonationRepository
    {
        MpDeposit GetDepositByProcessorTransferId(string processorTransferId);
        MpDonation GetDonationByTransactionCode(string transactionCode); // theoretically on settlement as transactionid
    }
}
