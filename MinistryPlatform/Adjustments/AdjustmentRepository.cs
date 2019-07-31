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
                "Journal_Entry_ID",
                "Created_Date",
                "Donation_Date",
                "Sent_To_GL_Date",
                "GL_Account_Number",
                "Amount",
                "Adjustment",
                "Description",
                "Donation_Distribution_ID"
            };
            //var filter = $"Sent_To_GL_Date IS NULL";

            var filters = new string[]
            {
                $"Sent_To_GL_Date IS NULL",
                $"Created_Date >= '{startDate:yyyy-MM-dd}'",
                $"Created_Date <= {endDate:sortable}"
            };

            var mpAdjustingJournalEntries = MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .WithSelectColumns(columns)
                .WithFilter(String.Join(" AND ", filters))
                .Build()
                .Search<MpDistributionAdjustment>();

            return mpAdjustingJournalEntries;
        }
    }
}
