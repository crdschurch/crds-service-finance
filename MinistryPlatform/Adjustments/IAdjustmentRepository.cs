using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MinistryPlatform.Models;

namespace MinistryPlatform.Adjustments
{
    public interface IAdjustmentRepository
    {
        List<MpAdjustingJournalEntry> GetAdjustmentsByDate(DateTime startDate, DateTime endDate);
    }
}
