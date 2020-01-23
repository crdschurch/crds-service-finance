using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
        
        public async Task<List<MpDonationDistribution>> GetByPledges(List<int> pledgeIds)
        {
            var token = await ApiUserRepository.GetApiClientTokenAsync("CRDS.Service.Finance");

            var columns = new string[] {
                "Pledge_ID_Table.[Pledge_ID]",
                "Donation_Distributions.[Donation_Distribution_ID]",
                "Donation_Distributions.[Amount]"
            };

            var filter = $"Pledge_ID_Table.[Pledge_ID] IN ({string.Join(",", pledgeIds)})";

            return (await MpRestBuilder.NewRequestBuilder()
                                .WithSelectColumns(columns)
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .BuildAsync()
                                .Search<MpDonationDistribution>()).ToList();
        }

        public async Task<List<MpDonationDistribution>> GetByDonationId(int donationId)
        {
            var token = await ApiUserRepository.GetApiClientTokenAsync("CRDS.Service.Finance");

            var columns = new string[] {
                "Donation_Distributions.[Donation_Distribution_ID]",
                "Donation_Distributions.[Donation_ID]",
                "Donation_Distributions.[Amount]",
                "Donation_Distributions.[Pledge_ID]",
                "Donation_Distributions.[Congregation_ID]",
                "Donation_Distributions.[HC_Donor_Congregation_ID]"
            };

            var filter = $"Donation_Distributions.[Donation_ID] = {donationId}";

            return (await MpRestBuilder.NewRequestBuilder()
                .WithSelectColumns(columns)
                .WithAuthenticationToken(token)
                .WithFilter(filter)
                .BuildAsync()
                .Search<MpDonationDistribution>()).ToList();
        }
        
        public async Task<List<MpDonationDistribution>> UpdateDonationDistributions(List<MpDonationDistribution> mpDonationDistributions)
        {
            var token = await ApiUserRepository.GetApiClientTokenAsync("CRDS.Service.Finance");

            return await MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .BuildAsync()
                .Update(mpDonationDistributions);
        }
    }
}
