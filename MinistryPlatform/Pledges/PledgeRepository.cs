using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MinistryPlatform.Repositories
{
    public class PledgeRepository : MinistryPlatformBase, IPledgeRepository
    {
        const int pledgeStatusActive = 1;
        const int pledgeStatusCompleted = 2;
        const int capitalCampaign = 1;

        public PledgeRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public async Task<List<MpPledge>> GetActiveAndCompleted(int contactId)
        {
            var token = await ApiUserRepository.GetApiClientTokenAsync("CRDS.Service.Finance");

            var columns = new string[] {
                "Pledge_Status_ID_Table.[Pledge_Status_ID]",
                "Pledges.[Pledge_ID]",
                "Pledges.[Total_Pledge]",
                "Pledge_Campaign_ID_Table.[Pledge_Campaign_ID]",
                "Pledge_Campaign_ID_Table.[Campaign_Name]",
                "Pledge_Campaign_ID_Table_Pledge_Campaign_Type_ID_Table.[Pledge_Campaign_Type_ID]",
                "Pledge_Campaign_ID_Table.[Start_Date] as [Campaign_Start_Date]",
                "Pledge_Campaign_ID_Table.[End_Date] as [Campaign_End_Date]",
                "Donor_ID_Table_Contact_ID_Table.[Contact_ID]",
                "Pledges.[First_Installment_Date]"
            };

            var filter = $"Pledge_Status_ID_Table.[Pledge_Status_ID] IN ({pledgeStatusActive},{pledgeStatusCompleted})";
            filter += $" AND Donor_ID_Table_Contact_ID_Table.[Contact_ID] = {contactId}";
            filter += $" AND Pledge_Campaign_ID_Table_Pledge_Campaign_Type_ID_Table.[Pledge_Campaign_Type_ID] = {capitalCampaign}";
            return await MpRestBuilder.NewRequestBuilder()
                                .WithSelectColumns(columns)
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .OrderBy("Pledge_Campaign_ID_Table.[Start_Date] DESC")
                                .BuildAsync()
                                .Search<MpPledge>();
        }
    }
}
