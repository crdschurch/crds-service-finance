using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public enum LogEventType
    {
        batchCreated,
        batchUpdated,
        creatingDeposit,
        depositCreated,
        depositExistsForSettlement,
        depositsAlreadyDeposited,
        depositsCreatedCount,
        depositSearchDateRange,
        depositsForSyncCount,
        donationNotFoundFail,
        donationNotFoundForTransaction,
        donationNotFoundRetry,
        incomingPushpayWebhook,
        newDepositToSync,
        noChargesForSettlement,
        noDepositsToSync,
        noRecurringGiftSubFound,
        previouslySyncedDeposit,
        refundingTransaction,
        stripeCancel,
        stripeCancelException
    }
}
