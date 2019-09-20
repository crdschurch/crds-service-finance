using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MinistryPlatform.Models
{
    // class for staging export value for MP Journal Entries
    public class MpJournalEntryStage
    {
        public string BatchNumber { get; set; }
        public Decimal TotalDebits { get; set; }
        public Decimal TotalCredits { get; set; }
        public DateTime BatchDate { get; set; }
        public XElement BatchData { get; set; }
        public int TransactionCount { get; set; }
    }
}
