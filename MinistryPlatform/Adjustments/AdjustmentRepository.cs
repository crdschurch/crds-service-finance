using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Models;

namespace MinistryPlatform.Adjustments
{
    public class AdjustmentRepository : MinistryPlatformBase, IAdjustmentRepository
    {
        public AdjustmentRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
        IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public List<MpDistributionAdjustment> GetAdjustmentsByDate(DateTime startDate, DateTime endDate)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");
            var columns = new string[] {
                "Distribution_Adjustment_ID",
                "Journal_Entry_ID",
                "Created_Date",
                "Donation_Date",
                "Processed_Date",
                "GL_Account_Number",
                "Amount",
                "Adjustment",
                "Description",
                "Donation_Distribution_ID"
            };

            var filters = new string[]
            {
                $"Processed_Date IS NULL",
                $"Created_Date >= '{startDate:yyyy-MM-dd}'",
                $"Created_Date <= '{endDate:yyyy-MM-dd}'"
            };

            var mpAdjustingJournalEntries = MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .WithSelectColumns(columns)
                .WithFilter(String.Join(" AND ", filters))
                .Build()
                .Search<MpDistributionAdjustment>();

            return mpAdjustingJournalEntries;
        }

        public void UpdateAdjustments(List<MpDistributionAdjustment> distributionAdjustments)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Update(distributionAdjustments, "cr_Distribution_Adjustments");
        }
    }
}
