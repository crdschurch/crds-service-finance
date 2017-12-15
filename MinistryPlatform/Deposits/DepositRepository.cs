using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;

namespace MinistryPlatform.Repositories
{
    public class DepositRepository : MinistryPlatformBase, IDepositRepository
    {
        public DepositRepository(IMinistryPlatformRestRequestBuilderFactory builder,
                               IApiUserRepository apiUserRepository,
                               IConfigurationWrapper configurationWrapper,
                               IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public MpDeposit CreateDeposit(MpDeposit mpDeposit)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Create(mpDeposit);
        }

        public MpDeposit GetDepositByProcessorTransferId(string processorTransferId)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            var columns = new string[] {
                "Deposit_ID"
            };
            var filter = $"Processor_Transfer_ID = '{processorTransferId}'";

            var deposits = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithSelectColumns(columns)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpDeposit>();

            return deposits.FirstOrDefault();
        }

        public List<MpDeposit> GetDepositsByTransferIds(List<string> transferIds)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            var filter = $"Processor_Transfer_ID IN (" + string.Join(',', transferIds ) + ")";

            var deposits = MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .WithFilter(filter)
                .Build()
                .Search<MpDeposit>();

            return deposits;
        }    
    }
}
