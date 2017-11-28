using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;

namespace MinistryPlatform
{
    public abstract class MinistryPlatformBase
    {
        protected readonly IMinistryPlatformRestRequestBuilderFactory MpRestBuilder;
        protected readonly IApiUserRepository ApiUserRepository;
        protected readonly IConfigurationWrapper ConfigurationWrapper;
        protected readonly IMapper Mapper;

        protected MinistryPlatformBase(IMinistryPlatformRestRequestBuilderFactory mpRestBuilder,
                                       IApiUserRepository apiUserRepository,
                                       IConfigurationWrapper configurationWrapper,
                                       IMapper mapper)
        {
            MpRestBuilder = mpRestBuilder;
            ApiUserRepository = apiUserRepository;
            ConfigurationWrapper = configurationWrapper;
            Mapper = mapper;
        }

    }
}
