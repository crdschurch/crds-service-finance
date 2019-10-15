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
        public List<VelosioJournalEntryBatch> CreateBatchPerUniqueJournalEntryBatchId(List<MpJournalEntry> journalEntries)
        {
            List<string> uniqueBatchIds = journalEntries.Select(jE => jE.BatchID).Distinct().ToList<string>();
            var batches = uniqueBatchIds
                .Select(batchId => new VelosioJournalEntryBatch(batchId)).ToList();

            return batches;
        }

        public void AddJournalEntryToAppropriateBatch(List<VelosioJournalEntryBatch> batches, MpJournalEntry journalEntry)
        {
            VelosioJournalEntryBatch batch = batches.Single(b => b.BatchNumber == journalEntry.BatchID);

            batch.BatchData.Add(SerializeJournalEntry(journalEntry));
            batch.TotalCredits += journalEntry.CreditAmount;
            batch.TotalDebits += journalEntry.DebitAmount;
            batch.TransactionCount++;
        }

        private XElement SerializeJournalEntry(MpJournalEntry mpJournalEntry)
        {
            var journalEntryXml = new XElement("BatchDataTable", null);
            journalEntryXml.Add(new XElement("BatchNumber", mpJournalEntry.BatchID));
            journalEntryXml.Add(new XElement("Reference", mpJournalEntry.GetReferenceString()));
            journalEntryXml.Add(new XElement("TransactionDate", DateTime.Now.Date));
            journalEntryXml.Add(new XElement("Account", mpJournalEntry.GL_Account_Number));
            journalEntryXml.Add(new XElement("DebitAmount", mpJournalEntry.DebitAmount));
            journalEntryXml.Add(new XElement("CreditAmount", mpJournalEntry.CreditAmount));

            return journalEntryXml;
        }
    }
}
