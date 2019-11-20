using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace Mock
{
    public class MpJournalEntryMock
    {
        public static MpJournalEntry Create() =>
            new MpJournalEntry
            {
                JournalEntryID = 2345678,
                CreatedDate = new DateTime(2019, 09, 03),
                ExportedDate = null,
                GL_Account_Number = "40001-325-02",
                BatchID = "CRJE20190903",
                CreditAmount = 10.0m,
                DebitAmount = 0.0m,
                Description = "Exported Item",
                AdjustmentYear = 2019,
                AdjustmentMonth = 9
            };

        public static List<MpJournalEntry> CreateList() =>
            new List<MpJournalEntry>
            {
                new MpJournalEntry
                {
                    JournalEntryID = 2345678,
                    CreatedDate = new DateTime(2019, 09, 03),
                    ExportedDate = null,
                    GL_Account_Number = "40001-325-02",
                    BatchID = "CRJE20190903",
                    CreditAmount = 10.0m,
                    DebitAmount = 0.0m,
                    Description = "Exported Item",
                    AdjustmentYear = 2019,
                    AdjustmentMonth = 9
                },
                new MpJournalEntry
                {
                    JournalEntryID = 2345678,
                    CreatedDate = new DateTime(2019, 09, 03),
                    ExportedDate = null,
                    GL_Account_Number = "40001-325-02",
                    BatchID = "CRJE20190903",
                    CreditAmount = 10.0m,
                    DebitAmount = 0.0m,
                    Description = "Exported Item",
                    AdjustmentYear = 2019,
                    AdjustmentMonth = 9
                },
                new MpJournalEntry
                {
                    JournalEntryID = 2345678,
                    CreatedDate = new DateTime(2019, 09, 03),
                    ExportedDate = null,
                    GL_Account_Number = "40001-325-02",
                    BatchID = "CRJE20190903",
                    CreditAmount = 10.0m,
                    DebitAmount = 0.0m,
                    Description = "Exported Item",
                    AdjustmentYear = 2019,
                    AdjustmentMonth = 9
                },
            };

        public static MpDonation CreateEmpty() => new MpDonation { };
    }
}
