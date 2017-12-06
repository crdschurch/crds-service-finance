using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using RestSharp;

namespace Crossroads.Service.Finance.Services
{
    public class PushpayPaymentProcessorService : IPaymentProcessorService
    {
        private readonly IRestClient _restClient;
        private readonly IPushpayService _pushpayService;
        private const int RequestsPerSecond = 10;
        private const int RequestsPerMinute = 60;

        public PushpayPaymentProcessorService(IRestClient restClient, IPushpayService pushpayService)
        {
            _restClient = restClient;
            _pushpayService = pushpayService;
        }

        // call out to the settlement/{settlementKey}/payments endpoint
        public PaymentsDto GetChargesForTransfer(string settlementKey)
        {
            var url = $"settlement/{settlementKey}/payments";
            var request = new RestRequest(url, Method.GET);
            var paymentsDto = _restClient.Execute<PaymentsDto>(request).Data;

            // determine the delay needed to avoid hitting the rate limits for Pushpay
            var delay = 0;
            var totalPages = paymentsDto.TotalPages;

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

                // call and parse next load
                url = $"settlement/{settlementKey}/payments?page={i}";
                request = new RestRequest(url, Method.GET);
                var response = _restClient.Execute<PaymentsDto>(request);
                paymentsDto.Payments.AddRange(response.Data.Payments);
            }

            return paymentsDto;
        }
    }
}
