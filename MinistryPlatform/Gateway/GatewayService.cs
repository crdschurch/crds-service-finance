using System;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using Crossroads.Web.Common.Security;
using MinistryPlatform;
using MinistryPlatform.Donors;
using MinistryPlatform.Interfaces;
using RestSharp;
using Utilities.Logging;

namespace Crossroads.Service.Finance.Services
{
    public class GatewayService: MinistryPlatformBase, IGatewayService
    {
        private readonly string gatewayEndpoint = Environment.GetEnvironmentVariable("CRDS_GATEWAY_CLIENT_ENDPOINT");
        private readonly string gatewayServiceKey = Environment.GetEnvironmentVariable("CRDS_GATEWAY_SERVICE_KEY");
        private readonly IRestClient _restClient;
        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly IDataLoggingService _dataLoggingService;

        public GatewayService(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper,
            IAuthenticationRepository authenticationRepository,
            IRecurringGiftRepository recurringGiftRepository,
            IDonorRepository donorRepository,
            IDataLoggingService dataLoggingService,
            IRestClient restClient = null) : base(builder, apiUserRepository, configurationWrapper, mapper)
        {
            _restClient = restClient ?? new RestClient(gatewayEndpoint);;
            _recurringGiftRepository = recurringGiftRepository;
            _donorRepository = donorRepository;
            _dataLoggingService = dataLoggingService;
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
                //var donor = _donorRepository.GetDonorByDonorId(stripeRecurringGift.DonorId);
                var tokenWithImpersonate = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

                var restRequest = new RestRequest($"api/donor/recurrence/{stripeRecurringGift.RecurringGiftId}?sendEmail=false", Method.DELETE);
                //restRequest.AddHeader("ImpersonateUserId", donor.EmailAddress);
                //restRequest.AddHeader("Authorization", tokenWithImpersonate);
                restRequest.AddHeader("GatewayServiceKey", gatewayServiceKey);

                Console.WriteLine($"Cancelling stripe recurring gift ({stripeSubscriptionId})");
                IRestResponse response = _restClient.Execute(restRequest);
                Console.WriteLine($"Status code: {response.StatusCode}");

                var stripeCancelEntry = new LogEventEntry(LogEventType.stripeCancel);
                stripeCancelEntry.Push("stripeCancelSubId", stripeSubscriptionId);
                stripeCancelEntry.Push("stripeCancelStatusCode", response.StatusCode);
                _dataLoggingService.LogDataEvent(stripeCancelEntry);
            }
            catch (Exception e)
            {
                Console.WriteLine($"CancelStripeRecurringGift error for stripe subscription id: {stripeSubscriptionId}");
                Console.WriteLine(e.Message);

                var stripeCancelExceptionEntry = new LogEventEntry(LogEventType.stripeCancelException);
                stripeCancelExceptionEntry.Push("stripeCancelSubId", stripeSubscriptionId);
                stripeCancelExceptionEntry.Push("stripeCancelException", e.Message);
                _dataLoggingService.LogDataEvent(stripeCancelExceptionEntry);
            }
        }
    }
}
