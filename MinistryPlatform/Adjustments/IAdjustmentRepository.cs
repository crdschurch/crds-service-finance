using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Models;

namespace MinistryPlatform.Adjustments
{
    public interface IAdjustmentRepository
    {
        Task<List<MpDistributionAdjustment>> GetUnprocessedDistributionAdjustments();
        Task UpdateAdjustments(List<MpDistributionAdjustment> distributionAdjustments);
    }
}
