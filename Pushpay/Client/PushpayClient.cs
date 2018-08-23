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
        private readonly string createAnticipatedPaymentsScope = "create_anticipated_payment";
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
            var tokenResponse = _pushpayTokenService.GetOAuthToken(donationsScope).Wait();
            _restClient.BaseUrl = apiUri;
            var request = new RestRequest(Method.GET)
            {
                Resource = $"settlement/{settlementKey}/payments"
            };
            request.AddParameter("Authorization", string.Format("Bearer " + tokenResponse.AccessToken), ParameterType.HttpHeader);

            var response = _restClient.Execute<PushpayPaymentsDto>(request);

            var paymentsDto = response.Data;

            // determine if we need to call again (multiple pages), then
            // determine the delay needed to avoid hitting the rate limits for Pushpay
            if (paymentsDto == null)
            {
                throw new Exception($"Get Settlement from Pushpay not successful: {response.Content}");
            }

            var totalPages = paymentsDto.TotalPages;

            if (totalPages > 1)
            {
                var delay = 0;
                if (totalPages >= RequestsPerSecond && totalPages < RequestsPerMinute)
                {
                    delay = 150;
                }
                else if (totalPages >= RequestsPerMinute)
                {
                    delay = 1000;
                }

                for (int i = 0; i < totalPages; i++)
                {
                    Thread.Sleep(delay);
                    request.Resource = $"settlement/{settlementKey}/payments?page={i}";
                    response = _restClient.Execute<PushpayPaymentsDto>(request);
                    paymentsDto.Items.AddRange(response.Data.Items);
                }
            }
            return paymentsDto;
        }

        public PushpayPaymentDto GetPayment(PushpayWebhook webhook)
        {
            _restClient.BaseUrl = new Uri(webhook.Events[0].Links.Payment);
            var token = _pushpayTokenService.GetOAuthToken(donationsScope).Wait().AccessToken;
            var request = new RestRequest(Method.GET);
            request.AddParameter("Authorization", string.Format("Bearer " + token), ParameterType.HttpHeader);

            var response = _restClient.Execute<PushpayPaymentDto>(request);

            var paymentDto = response.Data;

            // determine if we need to call again (multiple pages), then
            // determine the delay needed to avoid hitting the rate limits for Pushpay
            if (paymentDto == null)
            {
                throw new Exception($"Get Payment from Pushpay not successful: {response.Content}");
            }

            return paymentDto;
        }

        public List<PushpaySettlementDto> GetDepositsByDateRange(DateTime startDate, DateTime endDate)
        {
            var modStartDate = startDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            endDate = endDate.ToUniversalTime();

            var tokenResponse = _pushpayTokenService.GetOAuthToken(donationsScope).Wait();
            _restClient.BaseUrl = apiUri;
            var request = new RestRequest(Method.GET)
            {
                Resource = $"settlements?depositFrom={modStartDate}"
            };
            request.AddParameter("Authorization", string.Format("Bearer " + tokenResponse.AccessToken), ParameterType.HttpHeader);

            var response = _restClient.Execute<PushpaySettlementResponseDto>(request);

            var pushpayDepositDtos = response.Data.items;

            // determine if we need to call again (multiple pages), then
            // determine the delay needed to avoid hitting the rate limits for Pushpay
            if (pushpayDepositDtos == null)
            {
                throw new Exception($"Get Settlement from Pushpay not successful: {response.Content}");
            }

            var totalPages = response.Data.TotalPages;

            if (totalPages > 1)
            {
                var delay = 0;
                if (totalPages >= RequestsPerSecond && totalPages < RequestsPerMinute)
                {
                    delay = 150;
                }
                else if (totalPages >= RequestsPerMinute)
                {
                    delay = 1000;
                }

                for (int i = 0; i < totalPages; i++)
                {
                    Thread.Sleep(delay);
                    request.Resource = $"settlement/settlements?depositFrom={modStartDate}&page={i}";
                    response = _restClient.Execute<PushpaySettlementResponseDto>(request);
                    if (response.Data.items != null)
                    {
                        pushpayDepositDtos.AddRange(response.Data.items);
                    }
                    else
                    {
                        _logger.Warn($"No settlements found for start date {modStartDate} and page {i} in MP.");
                    }
                }
            }

            return pushpayDepositDtos;
        }

        public PushpayRecurringGiftDto GetRecurringGift(string resource)
        {
            var request = CreateRequest(resource, Method.GET, recurringGiftsScope);
            var response = _restClient.Execute<PushpayRecurringGiftDto>(request);
            return response.Data;
        }

        private RestRequest CreateRequest(string resource, Method method, string scope, object body = null)
        {
            _restClient.BaseUrl = apiUri;
            var request = new RestRequest(method)
            {
                Resource = resource
            };
            var tokenResponse = _pushpayTokenService.GetOAuthToken(scope).Wait();
            if (body != null)
            {
                request.AddHeader("Accept", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
            }
            request.AddParameter("Authorization", string.Format("Bearer " + tokenResponse.AccessToken), ParameterType.HttpHeader);
            return request;
        }

    }
}
