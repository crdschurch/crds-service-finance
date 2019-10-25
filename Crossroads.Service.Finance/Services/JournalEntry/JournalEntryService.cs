using MinistryPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crossroads.Service.Finance.Services.JournalEntry
{
    public class JournalEntryService : IJournalEntryService
    {
        public void AdjustExistingJournalEntry(MpDistributionAdjustment mpDistributionAdjustment, MpJournalEntry journalEntry)
        {
            if ( IsCredit(mpDistributionAdjustment) )
            {
                journalEntry.CreditAmount += mpDistributionAdjustment.Amount;
            }
            else
            {
                journalEntry.DebitAmount += Math.Abs(mpDistributionAdjustment.Amount);
            }
        }

        public MpJournalEntry CreateNewJournalEntry(string batchId, MpDistributionAdjustment mpDistributionAdjustment)
        {
            var mpJournalEntry = new MpJournalEntry
            {
                BatchID = batchId,
                CreatedDate = DateTime.Now,
                ExportedDate = null,
                Description = "test desc", // TODO: understand what goes here
                GL_Account_Number = mpDistributionAdjustment.GLAccountNumber,
                AdjustmentYear = mpDistributionAdjustment.DonationDate.Year,
                AdjustmentMonth = mpDistributionAdjustment.DonationDate.Month
            };

            if ( IsCredit(mpDistributionAdjustment) )
            {
                mpJournalEntry.CreditAmount = mpDistributionAdjustment.Amount;
            }
            else
            {
                mpJournalEntry.DebitAmount = Math.Abs(mpDistributionAdjustment.Amount);
            }

            return mpJournalEntry;
        }

        public MpJournalEntry NetCreditsAndDebits(MpJournalEntry journalEntry)
        {
            if ( IsNetCredit(journalEntry) )
            {
                journalEntry.CreditAmount -= journalEntry.DebitAmount;
                journalEntry.DebitAmount = 0;
            }
            else if ( IsNetDebit(journalEntry) )
            {
                journalEntry.DebitAmount -= journalEntry.CreditAmount;
                journalEntry.CreditAmount = 0;
            }

            return journalEntry;
        }

        public List<MpJournalEntry> RemoveWashEntries(List<MpJournalEntry> journalEntries)
        {
            List<MpJournalEntry> nonRedundantAdjustments = journalEntries.Where(e => e.CreditAmount != e.DebitAmount).ToList<MpJournalEntry>();
            return nonRedundantAdjustments;
        }

        private static bool IsNetDebit(MpJournalEntry journalEntry)
        {
            return journalEntry.DebitAmount > journalEntry.CreditAmount;
        }

        private static bool IsNetCredit(MpJournalEntry journalEntry)
        {
            return journalEntry.CreditAmount > journalEntry.DebitAmount;
        }

        private bool IsCredit(MpDistributionAdjustment mpDistributionAdjustment)
        {
            return Math.Sign(mpDistributionAdjustment.Amount) == 1;
        }
    }
}
