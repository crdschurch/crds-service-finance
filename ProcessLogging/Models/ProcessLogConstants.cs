using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessLogging.Models
{
    public class ProcessLogConstants
    {
        public enum MessageType
        {
            // Webhooks 
            donationCreatedWebhook,
            donationUpdatedWebhhook,
            recurringGiftCreatedWebhook,
            recurringGiftUpdatedWebhook,
            
            // Jobs
            jobStarting,
            jobDone,
            jobErrored,

            // Donations 
            gettingDonationDetails,
            refundedDonation,
            donationUpdated,
            donationNoSelectedSite,
            noNewDonationDetails,

            // Recurring Gifts 
            recurringGiftNoSelectedSiteUpdate,
            recurringGiftNoSelectedSiteCreate,
            recurringGiftNoSelectedSiteFromSync,
            createNewRecurringGiftFromSync,
            recurringGiftDateComparison,
            updatingExistingReferringGiftFromSync,
            recurringGiftsCreatedOrUpdated,
            creatingRecurringGift,
            recurringGiftsPulledButNotProcess,

            // Deposits
            noDepositsToSync,
            settlementsProcessed,
            pushpayDepositCount,
            depositsToSkip,
            depositsToCreate,
            creatingDeposit,
            depositAlreadyExists,
            noChargesForSettlement,
            batchCreated,
            batchDonationsUpdated,
            depositCreated,
            outOfBalanceBatch,

            // Impersonation
            impersonatingUser,
            impersonatedUserDonationHistory,

            // Journal
            createJournalEntries,
            exportJournalEntries,
            manualJournalEntryExport,
            systemJournalEntryExport,
            velosioExportResult
        }

        static Dictionary<string, string> _messageVersions = new Dictionary<string, string>
        {
            // Webhooks 
            {"donationCreatedWebhook", "1.0.0"},
            {"donationUpdatedWebhhook", "1.0.0"},
            {"recurringGiftCreatedWebhook", "1.0.0"},
            {"recurringGiftUpdatedWebhook", "1.0.0"},

            // Jobs 
            {"jobStarting", "1.0.0"},
            {"jobDone", "1.0.0"},
            {"jobErrored", "1.0.0"},
            
            // Donations
            {"gettingDonationDetails", "1.0.0"},
            {"refundedDonation", "1.0.0"},
            {"donationUpdated", "1.0.0"},
            {"donationNoSelectedSite", "1.0.0"},
            {"noNewDonationDetails", "1.0.0"},

            // Recurring Gifts
            {"recurringGiftNoSelectedSiteUpdate", "1.0.0"},
            {"recurringGiftNoSelectedSiteCreate", "1.0.0"},
            {"recurringGiftNoSelectedSiteFromSync", "1.0.0"},
            {"createNewRecurringGiftFromSync", "1.0.0"},
            {"recurringGiftDateComparison", "1.0.0"},
            {"updatingExistingReferringGiftFromSync", "1.0.0"},
            {"recurringGiftsCreatedOrUpdated", "1.0.0"},
            {"creatingRecurringGift", "1.0.0"},
            {"recurringGiftsPulledButNotProcess", "1.0.0"},

            // Deposits
            {"noDepositsToSync", "1.0.0"},
            {"settlementsProcessed", "1.0.0"},
            {"pushpayDepositCount", "1.0.0"},
            {"depositsToSkip", "1.0.0"},
            {"depositsToCreate", "1.0.0"},
            {"creatingDeposit", "1.0.0"},
            {"depositAlreadyExists", "1.0.0"},
            {"noChargesForSettlement", "1.0.0"},
            {"batchCreated", "1.0.0"},
            {"batchDonationsUpdated", "1.0.0"},
            {"depositCreated", "1.0.0"},
            {"outOfBalanceBatch", "1.0.0"},

            // Impersonation
            {"impersonatingUser", "1.0.0"},
            {"impersonatedUserDonationHistory", "1.0.0"},

            // Journal
            {"createJournalEntries", "1.0.0"},
            {"exportJournalEntries", "1.0.0"},
            {"manualJournalEntryExport", "1.0.0"},
            {"systemJournalEntryExport", "1.0.0"},
            {"velosioExportResult", "1.0.0"}
        };

        public static string GetMessageVersion(MessageType messageType)
        {
            return _messageVersions[messageType.ToString()];
        }
    }
}
