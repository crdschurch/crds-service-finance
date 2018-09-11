using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using Crossroads.Service.Finance.Models;
using log4net;
using Newtonsoft.Json;
using Pushpay.Models;
using Pushpay.Token;
using RestSharp;

namespace Pushpay.Client
{
    public class PushpayClient : IPushpayClient
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Uri apiUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_API_ENDPOINT") ?? "https://sandbox-api.pushpay.io/v1");
        private readonly string donationsScope = "read merchant:view_payments";
        private readonly string recurringGiftsScope = "merchant:view_recurring_payments";
        private readonly IPushpayTokenService _pushpayTokenService;
        private readonly IRestClient _restClient;
        private const int RequestsPerSecond = 10;
        private const int RequestsPerMinute = 60;

        public PushpayClient(IPushpayTokenService pushpayTokenService, IRestClient restClient = null)
        {
            _pushpayTokenService = pushpayTokenService;
            _restClient = restClient ?? new RestClient();
        }

        public PushpayPaymentsDto GetPushpayDonations(string settlementKey)
        {
            var resource = $"settlement/{settlementKey}/payments";
            var data = CreateAndExecuteRequest(apiUri, resource, Method.GET, donationsScope);
            return JsonConvert.DeserializeObject<PushpayPaymentsDto>(data);
        }

        public PushpayPaymentDto GetPayment(PushpayWebhook webhook)
        {
            var uri = new Uri(webhook.Events[0].Links.Payment);
            var data = CreateAndExecuteRequest(uri, null, Method.GET, donationsScope);
            return JsonConvert.DeserializeObject<PushpayPaymentDto>(data);
        }

        public List<PushpaySettlementDto> GetDepositsByDateRange(DateTime startDate, DateTime endDate)
        {
            var modStartDate = startDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            var resource = $"settlements?depositFrom={modStartDate}";
            var data = CreateAndExecuteRequest(apiUri, resource, Method.GET, donationsScope);
            return JsonConvert.DeserializeObject<List<PushpaySettlementResponseDto>>(data);
        }

        public PushpayRecurringGiftDto GetRecurringGift(string resource)
        {
            var data = CreateAndExecuteRequest(apiUri, resource, Method.GET, recurringGiftsScope);
            return JsonConvert.DeserializeObject<List<PushpayRecurringGiftDto>>(data);
        }

        private dynamic CreateAndExecuteRequest(Uri baseUri, string resourcePath, Method method, string scope, object body = null)
        {
            var request = new RestRequest(method);
            _restClient.BaseUrl = baseUri;
            if (resourcePath != null)
            {
                request.Resource = resourcePath;
            };
            var tokenResponse = _pushpayTokenService.GetOAuthToken(scope).Wait();
            if (body != null)
            {
                request.AddHeader("Accept", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
            }
            request.AddParameter("Authorization", string.Format("Bearer " + tokenResponse.AccessToken), ParameterType.HttpHeader);

            var response = _restClient.Execute<dynamic>(request);
            dynamic responseData;

            // pushpay has a different data return format depending on if you are calling for
            //  a specific payment (payment fields are at top level) vs. if you are calling
            //  for all payments (has page size, etc. at top level then items for actual payments data)

            if (response.Data.items != null)
            {
                // possible response where we have to call for additional pages to get all data
                responseData = JsonConvert.DeserializeObject<List<PushpayResponseBaseDto>>(response.Data);
            }
            else
            {
                // single entity response
                responseData = response.Data;
            }

            if (responseData.Items != null && responseData.Page < responseData.TotalPages)
            {
                // increment page first, as we already got the first page's data above
                for (int currentPage = response.Data.Page; currentPage < response.Data.TotalPages; ++currentPage)
                {
                    request.AddQueryParameter("page", currentPage.ToString());
                    var pageResponse = _restClient.Execute<PushpayResponseBaseDto>(request);
                    if (pageResponse.Data.items != null)
                    {
                        responseData.AddRange(pageResponse.Data.items);
                    }
                    else
                    {
                        Console.WriteLine($"No data in response: {resourcePath}");
                    }
                }
            }
            return responseData;
        }
    }
}
