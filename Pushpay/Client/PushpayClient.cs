﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using Pushpay.Models;
using Pushpay.Token;
using RestSharp;

namespace Pushpay.Client
{
    public class PushpayClient : IPushpayClient
    {
        private Uri apiUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_API_ENDPOINT") ?? "https://sandbox-api.pushpay.io/v1");
        private readonly string donationsScope = "read merchant:view_payments";
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
            if (paymentsDto == null) {
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

        public List<PushpaySettlementDto> GetDepositsByDateRange(DateTime startDate, DateTime endDate)
        {
            var tokenResponse = _pushpayTokenService.GetOAuthToken(donationsScope).Wait();
            _restClient.BaseUrl = apiUri;
            var request = new RestRequest(Method.GET)
            {
                Resource = $"settlements?startDate={startDate}&endDate={endDate}"
            };
            request.AddParameter("Authorization", string.Format("Bearer " + tokenResponse.AccessToken), ParameterType.HttpHeader);

            // TODO: Verify that this will give us a list or otherwise how this comes back
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
                    request.Resource = $"settlement/settlements?startDate={startDate}&endDate={endDate}?page={i}";
                    response = _restClient.Execute<PushpaySettlementResponseDto>(request);
                    pushpayDepositDtos.AddRange(response.Data.items);
                }
            }

            return pushpayDepositDtos;
        }
    }
}
