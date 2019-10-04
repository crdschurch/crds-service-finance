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

        public List<MpDistributionAdjustment> GetAdjustmentsByDate()
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

            var mpAdjustingJournalEntries = MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .WithSelectColumns(columns)
                .WithFilter("Processed_Date IS NULL")
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
