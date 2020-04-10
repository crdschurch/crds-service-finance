using MinistryPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinistryPlatform.JournalEntries;

namespace Crossroads.Service.Finance.Services.JournalEntry
{
    public class JournalEntryService : IJournalEntryService
    {
        private readonly IJournalEntryRepository _journalEntryRepository;

        public JournalEntryService(IJournalEntryRepository journalEntryRepository)
        {
            _journalEntryRepository = journalEntryRepository;
        }

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

        public async Task<List<MpJournalEntry>> AddBatchIdsAndClean(List<MpJournalEntry> journalEntries)
        {
            var journalEntryTasks = new List<Task>();

            foreach (var journalEntry in journalEntries)
            {
                journalEntryTasks.Add(NetCreditsAndDebits(journalEntry));
            }

            journalEntries.ForEach(e => NetCreditsAndDebits(e));

            var entries = journalEntries;
            var removeWashEntriesTask = Task.Run(() => RemoveWashEntries(entries));
            var washedEntries = await removeWashEntriesTask;

            washedEntries = await AddBatchIdsToJournalEntries(washedEntries);
            return washedEntries;
        }

        public Task<MpJournalEntry> NetCreditsAndDebits(MpJournalEntry journalEntry)
        {
            var netCreditsAndDebitsTask = Task.Run(() =>
            {
                if (IsNetCredit(journalEntry))
                {
                    journalEntry.CreditAmount -= journalEntry.DebitAmount;
                    journalEntry.DebitAmount = 0;
                }
                else if (IsNetDebit(journalEntry))
                {
                    journalEntry.DebitAmount -= journalEntry.CreditAmount;
                    journalEntry.CreditAmount = 0;
                }

                return journalEntry;
            });

            return netCreditsAndDebitsTask;
        }

        public List<MpJournalEntry> RemoveWashEntries(List<MpJournalEntry> journalEntries)
        {
            List<MpJournalEntry> nonRedundantAdjustments = journalEntries.Where(e => e.CreditAmount != e.DebitAmount).ToList<MpJournalEntry>();
            return nonRedundantAdjustments;
        }

        public async Task<List<MpJournalEntry>> AddBatchIdsToJournalEntries(List<MpJournalEntry> entries)
        {
            int batchNumber = 1;
            var today = DateTime.Now;

            // get highest batch number for the current day here and use when creating next batch ids
            var batchIds = await _journalEntryRepository.GetCurrentDateBatchIds();

            if (batchIds.Any())
            {
                var maxBatchId = batchIds.Max();
                batchNumber = int.Parse(maxBatchId.Substring(maxBatchId.Length - 3)) + 1;
            }

            var batchId = $"CRJE{today.Year}{today.Month}{today.Day}{batchNumber.ToString("000")}";

            foreach (MpJournalEntry journalEntry in entries)
            {
                journalEntry.BatchID = batchId;
            }

            return entries;
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
