using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using log4net;
using MinistryPlatform.Models;
using System.Collections.Generic;
using System.Reflection;

namespace MinistryPlatform.PledgeCampaigns
{
    public class PledgeCampaignRepository : MinistryPlatformBase, IPledgeCampaignRepository
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public PledgeCampaignRepository(IMinistryPlatformRestRequestBuilderFactory mpRestBuilder, IApiUserRepository apiUserRepository, IConfigurationWrapper configurationWrapper, IMapper mapper) : base(mpRestBuilder, apiUserRepository, configurationWrapper, mapper)
        {
        }

        public List<MpPledgeCampaign> GetCampaigns(int pledgeCampaignTypesId)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var columns = new string[] {
                "Pledge_Campaigns.[Pledge_Campaign_ID]",
                "Pledge_Campaigns.[Campaign_Name]",
                "Pledge_Campaigns.[Pledge_Campaign_Type_ID]",
            };

            var filter = $"Pledge_Campaigns.[Pledge_Campaign_Type_ID] = {pledgeCampaignTypesId}";
            return MpRestBuilder.NewRequestBuilder()
                                .WithSelectColumns(columns)
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpPledgeCampaign>();
        }
    }
}
