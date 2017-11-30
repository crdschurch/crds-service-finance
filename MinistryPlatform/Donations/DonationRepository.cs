using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Models;

namespace MinistryPlatform.Donations
{
    public class DonationRepository : MinistryPlatformBase, IDonationRepository
    {
        public DonationRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }


        public MpDeposit GetDepositByProcessorTransferId(string processorTransferId)
        {
            //var apiToken = ApiUserRepository.GetDefaultApiUserToken();
            //var searchString = $"Processor_Transfer_ID='{processorTransferId}'";

            var token = ApiUserRepository.GetDefaultApiUserToken();
            //var columns = new string[] {
            //    "Contact_ID",
            //    "Household_ID"
            //};
            var filter = $"Processor_Transfer_ID = {processorTransferId}";
            var deposits = MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                //.WithSelectColumns(columns)
                .WithFilter(filter)
                .Build()
                .Search<MpDeposit>();
            //if (!contacts.Any())
            //{
            //    throw new Exception($"No transfer found for transfer id: {processorTransferId}");
            //}
            return deposits.FirstOrDefault();

            //return _ministryPlatformRest.UsingAuthenticationToken(apiToken).Search<MpDeposit>(searchString).ToList().FirstOrDefault();
        }

        public MpDonation GetDonationByTransactionCode(string transactionCode)
        {
            // TODO: Implement this
            return null;
        }
    }
}
