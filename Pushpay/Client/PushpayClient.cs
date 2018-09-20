using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private int RateLimitCount = 0;

        public PushpayClient(IPushpayTokenService pushpayTokenService, IRestClient restClient = null)
        {
            _pushpayTokenService = pushpayTokenService;
            _restClient = restClient ?? new RestClient();
        }

        public List<PushpayPaymentDto> GetDonations(string settlementKey)
        {
            var resource = $"settlement/{settlementKey}/payments";
            var data = CreateAndExecuteRequest(apiUri, resource, Method.GET, donationsScope, null, true);
            return JsonConvert.DeserializeObject<List<PushpayPaymentDto>>(data);
        }

        public PushpayPaymentDto GetPayment(PushpayWebhook webhook)
        {
            var uri = new Uri(webhook.Events[0].Links.Payment);
            var data = CreateAndExecuteRequest(uri, null, Method.GET, donationsScope);
            return data == null ? null : JsonConvert.DeserializeObject<PushpayPaymentDto>(data);
        }

        public PushpayRecurringGiftDto GetRecurringGift(string resource)
        {
            var data = CreateAndExecuteRequest(apiUri, resource, Method.GET, recurringGiftsScope);
            return data == null ? null : JsonConvert.DeserializeObject<PushpayRecurringGiftDto>(data);
        }

        public List<PushpaySettlementDto> GetDepositsByDateRange(DateTime startDate, DateTime endDate)
        {
            var modStartDate = startDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            var resource = "settlements";
            Dictionary<string, string> queryParams = new Dictionary<string, string>()
            {
                { "depositFrom", modStartDate }
            };
            var data = CreateAndExecuteRequest(apiUri, resource, Method.GET, donationsScope, queryParams, true);
            return JsonConvert.DeserializeObject<List<PushpaySettlementDto>>(data);
        }

        private int GetRetrySeconds(int pushpayRetrySeconds)
        {
            // in case a bunch of requests come in at the same time, lets exponentially stagger the requests,
            //  and add a random number, so that they dont all run again at the same time
            var backoffSeconds = RateLimitCount * RateLimitCount;
            var randomSeconds = new Random().Next(1, 20);
            return pushpayRetrySeconds + backoffSeconds + randomSeconds;
        }

        // execute request, retry if rate limited
        private IRestResponse Execute(RestRequest request, string scope)
        {
            AddAuth(request, scope);
            var response = _restClient.Execute(request);
            if ((int)response.StatusCode == 429)
            {
                RateLimitCount++;
                var pushpayRetrySeconds = Convert.ToInt32(response.Headers.ToList().Find(x => x.Name == "Retry-After").Value);
                var retrySeconds = GetRetrySeconds(pushpayRetrySeconds);
                Console.WriteLine($"Hit rate limit. Sleeping for {retrySeconds} seconds");
                Thread.Sleep(retrySeconds * 1000);
                return Execute(request, scope);
            }
            RateLimitCount = 0;
            return response;
        }

        // execute request, retry if rate limited
        private IRestResponse<PushpayResponseBaseDto> ExecuteList(RestRequest request, string scope)
        {
            AddAuth(request, scope);
            var response = _restClient.Execute<PushpayResponseBaseDto>(request);
            if ((int)response.StatusCode == 429)
            {
                RateLimitCount++;
                var pushpayRetrySeconds = Convert.ToInt32(response.Headers.ToList().Find(x => x.Name == "Retry-After").Value);
                var retrySeconds = GetRetrySeconds(pushpayRetrySeconds);
                Console.WriteLine($"Hit rate limit. Sleeping for {retrySeconds} seconds");
                Thread.Sleep(retrySeconds * 1000);
                return ExecuteList(request, scope);
            }
            RateLimitCount = 0;
            return response;
        }

        private RestRequest AddAuth(RestRequest request, string scope)
        {
            var tokenResponse = _pushpayTokenService.GetOAuthToken(scope).Wait();
            // remove auth param, if exists
            var authParam = request.Parameters.FindIndex(x => x.Name == "Authorization");
            if (authParam > -1)
            {
                request.Parameters.RemoveAt(authParam);
            }
            request.AddParameter("Authorization", string.Format("Bearer " + tokenResponse.AccessToken), ParameterType.HttpHeader);
            return request;
        }

        private string CreateAndExecuteRequest(Uri baseUri, string resourcePath, Method method, string scope, Dictionary<string, string> queryParams = null, bool isList = false, object body = null)
        {
            var request = new RestRequest(method);
            _restClient.BaseUrl = baseUri;
            if (resourcePath != null)
            {
                request.Resource = resourcePath;
            };

            if (body != null)
            {
                request.AddHeader("Accept", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
            }
            if (queryParams != null)
            {
                foreach (KeyValuePair<string, string> entry in queryParams)
                {
                    // do something with entry.Value or entry.Key
                    request.AddQueryParameter(entry.Key, entry.Value);
                }
            }

            // pushpay has a different data return format depending on if you are calling for
            //  a specific payment (payment fields are at top level) vs. if you are calling
            //  for all payments (has page size, etc. at top level then items for actual payments data)
            // isList here refers to whether the resource will return a list (like all payments) or not (payment) for example
            if (!isList)
            {
                var response = Execute(request, scope);
                if ((int)response.StatusCode == 404 || response.ErrorException != null)
                {
                    Console.WriteLine(response.ErrorMessage);
                    return null;
                }
                return response.Content;
            }
            else
            {
                // data is possibly multiple pages
                var response = ExecuteList(request, scope);
                if (response.ErrorException != null)
                {
                    throw new Exception(response.ErrorMessage);
                }
                var responseDataItems = response.Data.items;

                // check if there are more pages that we have to get
                //  subtract one from Total Pages since Page is 0-index
                if (responseDataItems != null && response.Data.Page < response.Data.TotalPages - 1)
                {
                    // increment current page, as we already got the first page's data above
                    //  subtract one from Total Pages since currentPage is 0-index 
                    for (int currentPage = response.Data.Page + 1; currentPage < response.Data.TotalPages; currentPage++)
                    {
                        // remove page param, if exists
                        var pageParam = request.Parameters.FindIndex(x => x.Name == "page");
                        if (pageParam > -1)
                        {
                            request.Parameters.RemoveAt(pageParam);
                        }
                        request.AddParameter(new Parameter() { Name = "page", Value = currentPage.ToString(), Type = ParameterType.QueryString });
                        var pageResponse = ExecuteList(request, scope);
                        var pageItems = pageResponse.Data.items;
                        if (pageItems != null)
                        {
                            responseDataItems.AddRange(pageItems);
                        }
                        else
                        {
                            Console.WriteLine($"No data in response: {resourcePath}");
                        }
                    }
                    return JsonConvert.SerializeObject(responseDataItems);
                }
                return JsonConvert.SerializeObject(responseDataItems);
            }
        }
    }
}
