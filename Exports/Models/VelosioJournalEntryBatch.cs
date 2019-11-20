using System;
using System.Xml.Linq;

namespace Exports.Models
{
    public class VelosioJournalEntryBatch
    {
        public VelosioJournalEntryBatch(string batchId)
        {
            this.BatchNumber = batchId;
            this.TotalDebits = 0m;
            this.TotalCredits = 0m;
            this.BatchDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            this.TransactionCount = 0;
            this.BatchData = new XElement("BatchDataSet", null);
        }

        public string BatchNumber { get; set; }
        public Decimal TotalDebits { get; set; }
        public Decimal TotalCredits { get; set; }
        public DateTime BatchDate { get; set; }
        public int TransactionCount { get; set; }
        public XElement BatchData { get; set; }
    }
}