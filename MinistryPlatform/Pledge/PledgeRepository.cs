using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using log4net;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;

namespace MinistryPlatform.Repositories
{
    public class PledgeRepository : MinistryPlatformBase, IPledgeRepository
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        const int pledgeStatusActive = 1;
        const int pledgeStatusCompleted = 2;

        public PledgeRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public IList<MpPledge> GetActiveAndCompleted(int contactId)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            var columns = new string[] {
                "Pledge_Status_ID_Table.[Pledge_Status_ID]",
                "Pledges.[Pledge_ID]",
                "Pledges.[Total_Pledge]",
                "Pledge_Campaign_ID_Table.[Campaign_Name]",
                "Pledge_Campaign_ID_Table_Pledge_Campaign_Type_ID_Table.[Pledge_Campaign_Type_ID]",
                "Pledge_Campaign_ID_Table.[Campaign_Goal]",
                "Pledges.[Total_Pledge]",
                "Pledges.[Installments_Planned]",
                "Pledges.[Installments_Per_Year]",
                "Pledges.[_Installment_Amount]",
                "Pledges.[First_Installment_Date]",
                "Pledges.[_Last_Installment_Date]",
                "Donor_ID_Table_Contact_ID_Table.[Contact_ID]"
            };

            var filter = $"Pledge_Status_ID_Table.[Pledge_Status_ID] IN ({pledgeStatusActive},{pledgeStatusCompleted})";
            filter += $" AND Donor_ID_Table_Contact_ID_Table.[Contact_ID] = {contactId}";
            return MpRestBuilder.NewRequestBuilder()
                                .WithSelectColumns(columns)
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpPledge>();
        }
    }
}
