﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using log4net;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;

namespace MinistryPlatform.Repositories
{
    public class DonationDistributionRepository : MinistryPlatformBase, IDonationDistributionRepository
    {

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DonationDistributionRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }
        
        public List<MpDonationDistribution> GetByPledges(List<int> pledgeIds)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var columns = new string[] {
                "Pledge_ID_Table.[Pledge_ID]",
                "Donation_Distributions.[Donation_Distribution_ID]",
                "Donation_Distributions.[Amount]"
            };

            var filter = $"Pledge_ID_Table.[Pledge_ID] IN ({string.Join(",", pledgeIds)})";

            return MpRestBuilder.NewRequestBuilder()
                                .WithSelectColumns(columns)
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpDonationDistribution>().ToList();
        }
    }
}
