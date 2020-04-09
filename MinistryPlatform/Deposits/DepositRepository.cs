using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<MpDeposit> CreateDeposit(MpDeposit mpDeposit)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            return await MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .BuildAsync()
                .Create(mpDeposit);
        }

        public async Task<MpDeposit> GetDepositByProcessorTransferId(string processorTransferId)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var columns = new string[] {
                "Deposit_ID"
            };
            var filter = $"Processor_Transfer_ID = '{processorTransferId}'";

            var deposits = await MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithSelectColumns(columns)
                                .WithFilter(filter)
                                .BuildAsync()
                                .Search<MpDeposit>();

            return deposits.FirstOrDefault();
        }

        public async Task<List<MpDeposit>> GetByTransferIds(List<string> transferIds)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");
            var deposits = new List<MpDeposit>();

            // avoid hitting the 2000 character limit on MP REST API filter
            while (true)
            {
                if (transferIds.Count < 1)
                {
                    break;
                }

                var elementsToRemove = (transferIds.Count > 10) ? 10 : transferIds.Count;
                var transferIdSet = transferIds.Take(elementsToRemove).ToList();
                transferIds.RemoveRange(0, elementsToRemove);

                var filter = $"Processor_Transfer_ID IN (" + string.Join(',', transferIdSet) + ")";

                deposits.AddRange(await MpRestBuilder.NewRequestBuilder()
                    .WithAuthenticationToken(token)
                    .WithFilter(filter)
                    .BuildAsync()
                    .Search<MpDeposit>());
            }

            return deposits;
        }

        public async Task<List<MpDeposit>> GetByName(string depositName)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var filter = $"Deposit_Name LIKE '%{depositName}%'";

            var deposits = await MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .WithFilter(filter)
                .BuildAsync()
                .Search<MpDeposit>();

            return deposits;
        }
    }
}
