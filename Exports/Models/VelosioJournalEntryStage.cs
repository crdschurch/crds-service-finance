using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Exports.Models
{
    public class VelosioJournalEntryStage
    {
        public string BatchNumber { get; set; }
        public Decimal TotalDebits { get; set; }
        public Decimal TotalCredits { get; set; }
        public DateTime BatchDate { get; set; }
        public int TransactionCount { get; set; }
        public XElement BatchData { get; set; }
    }
}
