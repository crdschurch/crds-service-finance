using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;

namespace MinistryPlatform.Repositories
{
    public class BatchRepository : MinistryPlatformBase, IBatchRepository
    {
        public BatchRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public MpDonationBatch CreateDonationBatch(MpDonationBatch mpDonationBatch)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Create(mpDonationBatch);
        }

        public void UpdateDonationBatch(MpDonationBatch mpDonationBatch)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Update(mpDonationBatch);
            
        }
    }
}
