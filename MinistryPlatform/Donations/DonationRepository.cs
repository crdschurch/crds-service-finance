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

        public MpDonation GetDonationByTransactionCode(string transactionCode)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            var filter = $"Transaction_Code = {transactionCode}";
            var donations = MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .WithFilter(filter)
                .Build()
                .Search<MpDonation>();

            return donations.FirstOrDefault();
        }

        public List<MpDonation> UpdateDonations(List<MpDonation> donations)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Update(donations);
        }
    }
}
