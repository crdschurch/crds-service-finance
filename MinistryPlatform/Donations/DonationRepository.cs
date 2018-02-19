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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Repositories
{
    public class DonationRepository : MinistryPlatformBase, IDonationRepository
    {

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DonationRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public MpDonation GetDonationByTransactionCode(string transactionCode)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            var filter = $"Transaction_Code = '{transactionCode}'";
            var donations = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpDonation>();

            if(!donations.Any())
            {
                // TODO possibly refactor to create a more custom exception
                throw new Exception($"Donation does not exist for transaction code: {transactionCode}");
            }

            return donations.First();
        }

        public List<MpDonation> Update(List<MpDonation> donations)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();
            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Update(donations);
        }

        public MpDonation Update(MpDonation donation)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();
            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Update(donation);
        }

        public MpDonor CreateDonor(MpDonor donor)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();
            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Create(donor);
        }

        public MpDonorAccount CreateDonorAccount(MpDonorAccount donor)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();
            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Create(donor);
        }

        public void UpdateDonorAccount(JObject donorAccount)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            try
            {
                MpRestBuilder.NewRequestBuilder()
                    .WithAuthenticationToken(token)
                    .Build()
                    .Update(donorAccount, "Donor_Accounts");
            }
            catch (Exception e)
            {
                _logger.Error($"UpdateRecurringGift: Error updating recurring gift: {JsonConvert.SerializeObject(donorAccount)}", e);
            }
        }
    }
}
