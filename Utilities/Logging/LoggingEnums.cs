using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public enum LogEventType
    {
        stripeCancel, noDepositsToSync, depositsCreatedCount, incomingPushpayWebhook, donationNotFoundForTransaction,
        depositSearchDateRange, depositsForSyncCount, depositsAlreadyDeposited, newDepositToSync, previouslySyncedDeposit,
        creatingDeposit, depositExistsForSettlement, noChargesForSettlement, batchCreated, batchUpdated, depositCreated,
        noRecurringGiftSubFound, refundingTransaction, donationNotFoundRetry, donationNotFoundFail, stripeCancelException
    }
}
