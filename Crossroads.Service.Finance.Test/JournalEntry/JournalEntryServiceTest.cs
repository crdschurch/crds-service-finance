using Crossroads.Service.Finance.Services.JournalEntry;
using MinistryPlatform.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace Crossroads.Service.Finance.Test.JournalEntry
{
    public class JournalEntriesTest
    {
        private readonly IJournalEntryService _fixture;

        public JournalEntriesTest()
        {
            _fixture = new JournalEntryService();
        }

        [Fact]
        public void ShouldAddCreditAmountToAnExistingJournalEntry()
        {
            var sampleJournalEntry = CreateTestJournalEntry(20, 5000);
            var sampleAdjustment = CreateTestDistributionAdjustment(500);

            _fixture.AdjustExistingJournalEntry(sampleAdjustment, sampleJournalEntry);
            MpJournalEntry expectedAdjusted = CreateTestJournalEntry(20, 5500);

            Assert.Equal(expectedAdjusted.CreditAmount, sampleJournalEntry.CreditAmount);
            Assert.Equal(expectedAdjusted.DebitAmount, sampleJournalEntry.DebitAmount);
        }

        [Fact]
        public void ShouldNetCreditsAndDebitsWhenCreditAmountHigher() {
            var sampleJournalEntry = CreateTestJournalEntry(20, 5000);

            MpJournalEntry actualAdjusted = _fixture.NetCreditsAndDebits(sampleJournalEntry);
            MpJournalEntry expectedAdjusted = CreateTestJournalEntry(0, 4980);

            Assert.Equal(expectedAdjusted.CreditAmount, actualAdjusted.CreditAmount);
            Assert.Equal(expectedAdjusted.DebitAmount, actualAdjusted.DebitAmount);
        }

        [Fact]
        public void ShouldNotAdjustZeroViaNetMethodAmounts()
        {
            var sampleJournalEntry = CreateTestJournalEntry(0, 0);

            MpJournalEntry actualAdjusted = _fixture.NetCreditsAndDebits(sampleJournalEntry);
            MpJournalEntry expectedAdjusted = CreateTestJournalEntry(0, 0);

            Assert.Equal(expectedAdjusted.CreditAmount, actualAdjusted.CreditAmount);
            Assert.Equal(expectedAdjusted.DebitAmount, actualAdjusted.DebitAmount);
        }

        [Fact]
        public void ShouldStripOutAnyWashJournalEntries()
        {
            var sampleJournalEntry1 = CreateTestJournalEntry(0, 0);
            var sampleJournalEntry2 = CreateTestJournalEntry(1, 0);
            var sampleJournalEntry3 = CreateTestJournalEntry(0, 2);
            var sampleJournalEntry4 = CreateTestJournalEntry(5, 2);
            var sampleJournalEntry5 = CreateTestJournalEntry(5, 5);

            List<MpJournalEntry> journalEntries = new List<MpJournalEntry> { sampleJournalEntry1, sampleJournalEntry2,
                                                                             sampleJournalEntry3, sampleJournalEntry4,
                                                                             sampleJournalEntry4 };

            var adjusted = _fixture.RemoveWashEntries(journalEntries);

            var expectedCountWithoutWashEntries = 4;

            Assert.Equal(expectedCountWithoutWashEntries, adjusted.Count);
        }

        private MpJournalEntry CreateTestJournalEntry(decimal debit, decimal credit) {
            var mpJournalEntry = new MpJournalEntry
            {
                BatchID = "123",
                CreatedDate = DateTime.Now,
                ExportedDate = null,
                Description = "test desc",
                GL_Account_Number = "123",
                AdjustmentYear = new DateTime(2019, 08, 01).Year,
                AdjustmentMonth = new DateTime(2019, 08, 01).Month,
                CreditAmount = credit,
                DebitAmount = debit
            };

            return mpJournalEntry;
        }

        private MpDistributionAdjustment CreateTestDistributionAdjustment(int amount) {
            return new MpDistributionAdjustment
            {
                GLAccountNumber = "40001-325-02",
                DonationDate = new DateTime(2019, 08, 01),
                Amount = amount
            };
        }
        
    }
}
