using Crossroads.Service.Finance.Models;
using MinistryPlatform.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLog;
using Pushpay.Models;
using Pushpay.Token;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pushpay.Client
{
    public class PushpayClient : IPushpayClient
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Uri apiUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_API_ENDPOINT") ?? "https://sandbox-api.pushpay.io/v1");
        private readonly string donationsScope = "read merchant:view_payments";
        private readonly string recurringGiftsScope = "merchant:view_recurring_payments";
        private readonly IPushpayTokenService _pushpayTokenService;
        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IRestClient _restClient;
        private const int RequestsPerSecond = 10;
        private const int RequestsPerMinute = 60;
        // rate limit count may not be accurate as this is global
        //  and potential for multiple threads to interact with this
        private int RateLimitCount = 0;

        public PushpayClient(IPushpayTokenService pushpayTokenService, IRecurringGiftRepository recurringGiftRepository, IRestClient restClient = null)
        {
            _pushpayTokenService = pushpayTokenService;
            _recurringGiftRepository = recurringGiftRepository;
            _restClient = restClient ?? new RestClient();
            _restClient.BaseUrl = apiUri;
        }

        public List<PushpayPaymentDto> GetDonations(string settlementKey)
        {
            var resource = $"settlement/{settlementKey}/payments";
            var data = CreateAndExecuteRequest(resource, Method.GET, donationsScope, null, true).Result;
            return JsonConvert.DeserializeObject<List<PushpayPaymentDto>>(data);
        }

        public async Task<PushpayPaymentDto> GetPayment(PushpayWebhook webhook)
        {
            var uri = webhook.Events[0].Links.Payment;
            var data = await CreateAndExecuteRequest(uri, Method.GET, donationsScope);
            return data == null ? null : JsonConvert.DeserializeObject<PushpayPaymentDto>(data);
        }

        public async Task<PushpayRecurringGiftDto> GetRecurringGift(string resource)
        {
            var data = await CreateAndExecuteRequest(resource, Method.GET, recurringGiftsScope);
            return data == null ? null : JsonConvert.DeserializeObject<PushpayRecurringGiftDto>(data);
        }

        public async Task<List<PushpaySettlementDto>> GetDepositsByDateRange(DateTime startDate, DateTime endDate)
        {
            var modStartDate = startDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            var resource = "settlements";
            List<QueryParameter> queryParams = new List<QueryParameter>()
            {
                new QueryParameter("depositFrom", modStartDate)
            };

            var data = await CreateAndExecuteRequest(resource, Method.GET, donationsScope, queryParams, true);
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

        public List<PushpayRecurringGiftDto> GetNewAndUpdatedRecurringGiftsByDateRange(DateTime startDate, DateTime endDate)
        {
            var exceptionMessage = string.Empty;

            try
            {
                var modStartDate = startDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                var modEndDate = endDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                var merchantKey = Environment.GetEnvironmentVariable("PUSHPAY_MERCHANT_KEY");

                var resource = $"merchant/{merchantKey}/recurringpayments";
                List<QueryParameter> queryParams = new List<QueryParameter>()
                {
                    new QueryParameter("updatedFrom", modStartDate),
                    new QueryParameter("updatedTo", modEndDate),
                    new QueryParameter("status", "active"),
                    new QueryParameter("status", "paused"),
                    new QueryParameter("status", "cancelled"),
                    new QueryParameter("pageSize", "100")
                };

                var data = CreateAndExecuteRequest(resource, Method.GET, recurringGiftsScope, queryParams, true).Result;

                if (data == null)
                {
                    _logger.Error("Null data in GetNewAndUpdatedRecurringGiftsByDateRange");
                    Console.WriteLine("Null data in GetNewAndUpdatedRecurringGiftsByDateRange");
                }

                var settings = new JsonSerializerSettings
                {
                    Error = delegate (object sender, ErrorEventArgs args)
                    {
                        _logger.Error($"Error in deserializing recurring gifts: {args.ErrorContext.Error.Message}");
                        Console.WriteLine(args.ErrorContext.Error.Message);
                        args.ErrorContext.Handled = true;
                    }};

                var newData = JArray.Parse(data);

                var recurringGifts = new List<PushpayRecurringGiftDto>();

                foreach (var item in newData)
                {
                    try
                    {
                        var gift = JsonConvert.DeserializeObject<PushpayRecurringGiftDto>(item.ToString());
                        recurringGifts.Add(gift);
                    }
                    catch (Exception e)
                    {
                        //TODO: This is a temporary usage of CreateRawPushpayRecurrentGiftSchedule
                        //TODO: When refactoring this process, we will log all raw recurring data, then process from the raw data.
                        _recurringGiftRepository.CreateRawPushpayRecurrentGiftSchedule(item.ToString());
                        _logger.Error($"Could not parse recurring gift: {e.Message}, {item.ToString()}");
                    }
                }

                return recurringGifts;

            }
            catch (Exception e)
            {
                _logger.Error($"Error in GetNewAndUpdatedRecurringGiftsByDateRange: {e.Message}", e.ToString());
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }

            throw new Exception(exceptionMessage);
        }

        public async Task<List<string>> GetRecurringGiftsAsync(DateTime startDate, DateTime endDate)
        {
            var recurringGiftRequest = await CreateRecurringGiftRequestAsync(startDate, endDate);
            var responseContent = await ExecuteRecurringGiftsRequestAsync(recurringGiftRequest);
            return await ParseRawRecurringGiftsJson(responseContent);
        }

        private static object ToObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Children<JProperty>()
                        .ToDictionary(prop => prop.Name,
                            prop => ToObject(prop.Value),
                            StringComparer.OrdinalIgnoreCase);

                case JTokenType.Array:
                    return token.Select(ToObject).ToList();

                default:
                    return ((JValue)token).Value;
            }
        }

        public async Task<List<PushpayPaymentDto>> GetPolledDonations(DateTime startTime, DateTime endTime)
        {
            var modStartDate = startTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            var modEndDate = endTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            var merchantKey = Environment.GetEnvironmentVariable("PUSHPAY_MERCHANT_KEY");

            var resource = $"merchant/{merchantKey}/payments";
            List<QueryParameter> queryParams = new List<QueryParameter>()
            {
                new QueryParameter("updatedFrom", modStartDate),
                new QueryParameter("updatedTo", modEndDate),
                new QueryParameter("pageSize", "100")
            };
            var data = await CreateAndExecuteRequest(resource, Method.GET, donationsScope, queryParams, true);
            var payments = JsonConvert.DeserializeObject<List<PushpayPaymentDto>>(data);
            return payments;
        }

        // execute request, retry if rate limited
        private async Task<IRestResponse> Execute(RestRequest request, string scope)
        {
            await AddAuth(request, scope);
            var response = _restClient.Execute(request);
            if ((int)response.StatusCode == 429)
            {
                RateLimitCount++;
                var pushpayRetrySeconds = Convert.ToInt32(response.Headers.ToList().Find(x => x.Name == "Retry-After").Value);
                var retrySeconds = GetRetrySeconds(pushpayRetrySeconds);
                Console.WriteLine($"Hit rate limit. Sleeping for {retrySeconds} seconds");
                Thread.Sleep(retrySeconds * 1000);
                return await Execute(request, scope);
            }
            RateLimitCount = 0;
            return response;
        }

        // execute request, retry if rate limited
        private async Task<IRestResponse<PushpayResponseBaseDto>> ExecuteList(RestRequest request, string scope)
        {
            await AddAuth(request, scope);
            var response = _restClient.Execute<PushpayResponseBaseDto>(request);
            if ((int)response.StatusCode == 429)
            {
                RateLimitCount++;
                var pushpayRetrySeconds = Convert.ToInt32(response.Headers.ToList().Find(x => x.Name == "Retry-After").Value);
                var retrySeconds = GetRetrySeconds(pushpayRetrySeconds);
                Console.WriteLine($"Hit rate limit. Sleeping for {retrySeconds} seconds");
                Thread.Sleep(retrySeconds * 1000);
                return await ExecuteList(request, scope);
            }
            RateLimitCount = 0;
            return response;
        }

        private async Task<RestRequest> AddAuth(RestRequest request, string scope)
        {
            var tokenResponse = await _pushpayTokenService.GetOAuthToken(scope);
            // remove auth param, if exists
            var authParam = request.Parameters.FindIndex(x => x.Name == "Authorization");
            if (authParam > -1)
            {
                request.Parameters.RemoveAt(authParam);
            }
            request.AddParameter("Authorization", string.Format("Bearer " + tokenResponse.AccessToken), ParameterType.HttpHeader);
            return request;
        }

        private async Task<string> CreateAndExecuteRequest(string uriOrResource, Method method, string scope, List<QueryParameter> queryParams = null, bool isList = false, object body = null)
        {
            var request = new RestRequest(method)
            {
                Resource = uriOrResource.StartsWith(apiUri.AbsoluteUri, StringComparison.Ordinal) ? uriOrResource.Replace(apiUri.AbsoluteUri, "") : uriOrResource
            };

            if (body != null)
            {
                request.AddHeader("Accept", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
            }
            if (queryParams != null)
            {
                foreach (QueryParameter entry in queryParams)
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
                var response = await Execute(request, scope);
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
                var response = await ExecuteList(request, scope);
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
                        var pageResponse = await ExecuteList(request, scope);
                        var pageItems = pageResponse.Data.items;
                        if (pageItems != null)
                        {
                            responseDataItems.AddRange(pageItems);
                        }
                        else
                        {
                            _logger.Error($"No data in response: {request.Resource}");
                            Console.WriteLine($"No data in response: {request.Resource}");
                        }
                    }
                    return JsonConvert.SerializeObject(responseDataItems);
                }
                return JsonConvert.SerializeObject(responseDataItems);
            }
        }

        private async Task<RestRequest> CreateRecurringGiftRequestAsync(DateTime startDate, DateTime endDate)
        {
            _logger.Info($"CreateRecurringGiftsRequestAsync is starting.  Start Date: {startDate}, End Date: {endDate}");

            var merchantKey = Environment.GetEnvironmentVariable("PUSHPAY_MERCHANT_KEY");
            var resource = $"merchant/{merchantKey}/recurringpayments";
            var request = new RestRequest(Method.GET)
            {
                Resource = resource.StartsWith(apiUri.AbsoluteUri, StringComparison.Ordinal) ? resource.Replace(apiUri.AbsoluteUri, "") : resource
            }; 
            
            var modStartDate = startDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            var modEndDate = endDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            _logger.Info($"Date range we are giving to pushpay. Start Date: {modStartDate}, End Date: {modEndDate}");
            List<QueryParameter> queryParams = new List<QueryParameter>()
            {
                new QueryParameter("updatedFrom", modStartDate),
                new QueryParameter("updatedTo", modEndDate),
                new QueryParameter("status", "active"),
                new QueryParameter("status", "paused"),
                new QueryParameter("status", "cancelled"),
                new QueryParameter("pageSize", "100")
            };
            
            foreach (QueryParameter param in queryParams)
            {
                request.AddQueryParameter(param.Key, param.Value);
            }

            return request;
        }

        private async Task<string> ExecuteRecurringGiftsRequestAsync(RestRequest recurringGiftRequest)
        {
            _logger.Info($"ExecuteRecurringGiftsRequestAsync is starting.");

            // data is possibly multiple pages
            int currentPage = 0;
            int totalPages = 0;
            bool hasItems = false;
            var responseDataItems = new List<object>();

            do
            {
                var response = await ExecuteList(recurringGiftRequest, recurringGiftsScope);
                if (response.ErrorException != null)
                {
                    throw new Exception(response.ErrorMessage);
                }
                
                hasItems = response.Data.items != null;

                if (hasItems)
                {
                    responseDataItems.AddRange(response.Data.items);
                }
                else
                {
                    _logger.Error($"No data in response: {recurringGiftRequest.Resource}");
                    Console.WriteLine($"No data in response: {recurringGiftRequest.Resource}");
                }

                // remove page param, if exists
                var pageParam = recurringGiftRequest.Parameters.FindIndex(x => x.Name == "page");
                if (pageParam > -1)
                {
                    recurringGiftRequest.Parameters.RemoveAt(pageParam);
                }
                currentPage = response.Data.Page + 1;  // Add one to the current page to read the next page
                totalPages = response.Data.TotalPages - 1;  //TotalPages is 0 indexed
                recurringGiftRequest.AddParameter(new Parameter() { Name = "page", Value = currentPage.ToString(), Type = ParameterType.QueryString });                
            } while (hasItems && currentPage <= totalPages);

            return JsonConvert.SerializeObject(responseDataItems);
        }

        private async Task<List<string>> ParseRawRecurringGiftsJson(string rawJson)
        {
            if (rawJson == null)
            {
                _logger.Error("Null data in GetNewAndUpdatedRecurringGiftsByDateRange");
                Console.WriteLine("Null data in GetNewAndUpdatedRecurringGiftsByDateRange");
            }

            var newData = JArray.Parse(rawJson);

            var recurringGifts = new List<string>();

            foreach (var item in newData)
            {
                try
                {
                    var recurringGift = item.ToString();
                    recurringGifts.Add(recurringGift);
                }
                catch (Exception e)
                {
                    _logger.Error($"Could not parse recurring gift: {e.Message}, {item.ToString()}");
                }
            }

            return recurringGifts;
        }

        public async Task<List<string>> GetPolledDonationsJson(DateTime startTime, DateTime endTime)
        {
	        var modStartDate = startTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
	        var modEndDate = endTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
	        var merchantKey = Environment.GetEnvironmentVariable("PUSHPAY_MERCHANT_KEY");

	        var resource = $"merchant/{merchantKey}/payments";
	        List<QueryParameter> queryParams = new List<QueryParameter>()
	        {
		        new QueryParameter("updatedFrom", modStartDate),
		        new QueryParameter("updatedTo", modEndDate),
		        new QueryParameter("pageSize", "100")
	        };
	        var data = await CreateAndExecuteRequest(resource, Method.GET, donationsScope, queryParams, true);

	        var newData = JArray.Parse(data);

	        var donations = new List<string>();

	        foreach (var item in newData)
	        {
		        try
		        {
			        var donation = item.ToString();
			        donations.Add(donation);
		        }
		        catch (Exception e)
		        {
			        _logger.Error($"Could not parse donation: {e.Message}, {item.ToString()}");
		        }
	        }

	        return donations;
        }
    }
}
