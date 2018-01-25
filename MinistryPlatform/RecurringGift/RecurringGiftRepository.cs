using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;

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

            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Create(mpRecurringGift);
        }
    }
}
