using System;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Newtonsoft.Json;

namespace MinistryPlatform.Repositories
{
    public class RecurringGiftRepository : MinistryPlatformBase, IRecurringGiftRepository
    {
        public RecurringGiftRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public MpRecurringGift CreateRecurringGift(MpRecurringGift mpRecurringGift)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            try
            {
                return MpRestBuilder.NewRequestBuilder()
                    .WithAuthenticationToken(token)
                    .Build()
                    .Create(mpRecurringGift);
            }
            catch (Exception e)
            {
                // TODO log
                return null;
            }
        }
    }
}
