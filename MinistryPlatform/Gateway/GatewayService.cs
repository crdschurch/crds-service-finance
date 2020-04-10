using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using Crossroads.Web.Common.Security;
using MinistryPlatform;
using MinistryPlatform.Donors;
using MinistryPlatform.Interfaces;
using RestSharp;
using System;

namespace Crossroads.Service.Finance.Services
{
    public class GatewayService: MinistryPlatformBase, IGatewayService
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string _gatewayEndpoint = Environment.GetEnvironmentVariable("CRDS_GATEWAY_CLIENT_ENDPOINT");
        private readonly string _gatewayServiceKey = Environment.GetEnvironmentVariable("CRDS_GATEWAY_SERVICE_KEY");
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
            _restClient = restClient ?? new RestClient(_gatewayEndpoint);;
            _recurringGiftRepository = recurringGiftRepository;
            _donorRepository = donorRepository;
        }

        /*
         * This will cancel a stripe recurring gift by it's subscription id
         * This is done by calling crds-angular gateway code and impersonating
         * the user to do so. This is scheduled to be used for the first few months
         * of the pushpay migration for credit cards migrations.
         */
        public async void CancelStripeRecurringGift(string stripeSubscriptionId)
        {
            try
            {
                var stripeRecurringGift = await _recurringGiftRepository.FindRecurringGiftBySubscriptionId(stripeSubscriptionId);
                var tokenWithImpersonate = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

                var restRequest = new RestRequest($"api/donor/recurrence/{stripeRecurringGift.RecurringGiftId}?sendEmail=false", Method.DELETE);
                restRequest.AddHeader("GatewayServiceKey", _gatewayServiceKey);

                Console.WriteLine($"Cancelling stripe recurring gift ({stripeSubscriptionId})");
                IRestResponse response = _restClient.Execute(restRequest);
                Console.WriteLine($"Status code: {response.StatusCode}");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error in GatewayService.CancelStripeRecurringGift for Stripe subscription={stripeSubscriptionId}: {ex.Message}");
                _logger.Error(ex, $"Error in GatewayService.CancelStripeRecurringGift for Stripe subscription={stripeSubscriptionId}: {ex.Message}");
            }
        }
    }
}
