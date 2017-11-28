using System.Collections.Generic;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Models;
using MinistryPlatform.Interfaces;
using System.Linq;
using System;
using AutoMapper;

namespace MinistryPlatform.Repositories
{
    public class OpportunityRepository : MinistryPlatformBase, IOpportunityRepository
    {
        public OpportunityRepository(IMinistryPlatformRestRequestBuilderFactory builder,
                               IApiUserRepository apiUserRepository,
                               IConfigurationWrapper configurationWrapper,
                               IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) {}

        public MpOpportunity GetOpportunity(int opportunityId)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();
            var columns = new string[] {
                "Opportunity_ID",
                "Opportunity_Title",
                "Add_to_Group as [Group_ID]",
                "Shift_Start",
                "Shift_End",
                "Room"
            };
            var filter = $"Opportunity_ID = {opportunityId}";
            var opportunities = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithSelectColumns(columns)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpOpportunity>();
            if(!opportunities.Any()) {
                throw new Exception($"No opportunity found: {opportunityId}");
            }
            return opportunities.FirstOrDefault();
        }
    }
}
