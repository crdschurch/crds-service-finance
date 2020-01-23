using Exports.Models;
using MinistryPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Crossroads.Service.Finance.Services.JournalEntryBatch
{
    public class JournalEntryBatchService : IJournalEntryBatchService
    {
        public async Task<List<VelosioJournalEntryBatch>> CreateBatchPerUniqueJournalEntryBatchId(List<MpJournalEntry> journalEntries)
        {
            List<string> uniqueBatchIds = journalEntries.Select(jE => jE.BatchID).Distinct().ToList<string>();
            var batches = uniqueBatchIds
                .Select(batchId => new VelosioJournalEntryBatch(batchId)).ToList();

            return batches;
        }

        public void AddJournalEntryToAppropriateBatch(List<VelosioJournalEntryBatch> batches, MpJournalEntry journalEntry, string sharedBatchIdForDisplay)
        {
            VelosioJournalEntryBatch batch = batches.Single(b => b.BatchNumber == journalEntry.BatchID);
            batch.BatchDate = GetLastDayOfMonthDonationWasMadeIn(journalEntry);
            batch.BatchData.Add(SerializeJournalEntry(journalEntry, batch, sharedBatchIdForDisplay));
            batch.TotalCredits += journalEntry.CreditAmount;
            batch.TotalDebits += journalEntry.DebitAmount;
            batch.TransactionCount++;
        }

        private XElement SerializeJournalEntry(MpJournalEntry mpJournalEntry, VelosioJournalEntryBatch batch, string sharedBatchIdForDisplay)
        {
            var journalEntryXml = new XElement("BatchDataTable", null);
            journalEntryXml.Add(new XElement("BatchNumber", sharedBatchIdForDisplay));
            journalEntryXml.Add(new XElement("Reference", mpJournalEntry.GetReferenceString()));
            journalEntryXml.Add(new XElement("TransactionDate", 
                                             GetLastDayOfMonthDonationWasMadeIn(mpJournalEntry).Date.ToShortDateString()));
            journalEntryXml.Add(new XElement("Account", mpJournalEntry.GL_Account_Number));
            journalEntryXml.Add(new XElement("DebitAmount", mpJournalEntry.DebitAmount));
            journalEntryXml.Add(new XElement("CreditAmount", mpJournalEntry.CreditAmount));

            return journalEntryXml;
        }
        private DateTime GetLastDayOfMonthDonationWasMadeIn(MpJournalEntry journalEntry)
        {
            int daysInMonthOfDonation = DateTime.DaysInMonth(journalEntry.AdjustmentYear, journalEntry.AdjustmentMonth);
            DateTime endOfMonth = new DateTime(journalEntry.AdjustmentYear,
                                               journalEntry.AdjustmentMonth,
                                               daysInMonthOfDonation);

            return endOfMonth;
        }
    }
}
