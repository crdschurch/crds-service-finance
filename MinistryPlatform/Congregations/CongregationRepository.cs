using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Models;

namespace MinistryPlatform.Congregations
{
    public class CongregationRepository : MinistryPlatformBase, ICongregationRepository
    {
        public CongregationRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }


        public List<MpCongregation> GetCongregationByCongregationName(string congregationName)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var selectColumns = new string[] {
                "Congregations.[Congregation_ID]",
                "Congregations.[Congregation_Name]"
            };

            var filter = $"Congregations.Congregation_Name = '{congregationName}'";

            return MpRestBuilder.NewRequestBuilder()
                .WithSelectColumns(selectColumns)
                .WithAuthenticationToken(token)
                .WithFilter(filter)
                .Build()
                .Search<MpCongregation>().ToList();
        }
    }
}
