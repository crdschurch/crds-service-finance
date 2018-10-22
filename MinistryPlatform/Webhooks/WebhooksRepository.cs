using System;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;

namespace MinistryPlatform.Repositories
{
    public class WebhooksRepository : MinistryPlatformBase, IWebhooksRepository
    {
        public WebhooksRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper)
        {
        }

        public void Create(MpPushpayWebhook webhookData)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");
            MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Create(webhookData);
        }
    }
}
