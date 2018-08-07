using System;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using Crossroads.Web.Common.Security;
using MinistryPlatform;
using MinistryPlatform.Donors;
using MinistryPlatform.Interfaces;
using RestSharp;

namespace Crossroads.Service.Finance.Services
{
    public class GatewayService: MinistryPlatformBase, IGatewayService
    {
        private readonly string gatewayEndpoint = Environment.GetEnvironmentVariable("CRDS_GATEWAY_CLIENT_ENDPOINT");
        private readonly IRestClient _restClient;
        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IDonorRepository _donorRepository;

        public GatewayService(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper,
            IAuthenticationRepository authenticationRepository,
            IRecurringGiftRepository recurringGiftRepository,
            IDonorRepository donorRepository,
            IRestClient restClient = null) : base(builder, apiUserRepository, configurationWrapper, mapper)
        {
            _restClient = restClient ?? new RestClient(gatewayEndpoint);;
            _recurringGiftRepository = recurringGiftRepository;
            _donorRepository = donorRepository;
        }

        /*
         * This will cancel a stripe recurring gift by it's subscription id
         * This is done by calling crds-angular gateway code and impersonating
         * the user to do so. This is scheduled to be used for the first few months
         * of the pushpay migration for credit cards migrations.
         * 
         */
        public void CancelStripeRecurringGift(string stripeSubscriptionId)
        {
            var stripeRecurringGift = _recurringGiftRepository.FindRecurringGiftBySubscriptionId(stripeSubscriptionId);
            var donor = _donorRepository.GetDonorByDonorId(stripeRecurringGift.DonorId);
            var tokenWithImpersonate = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var restRequest = new RestRequest($"api/donor/recurrence/{stripeRecurringGift.RecurringGiftId}", Method.DELETE);
            restRequest.AddHeader("ImpersonateUserId", donor.EmailAddress);
            restRequest.AddHeader("Authorization", tokenWithImpersonate);

            IRestResponse response = _restClient.Execute(restRequest);
        }
    }
}
