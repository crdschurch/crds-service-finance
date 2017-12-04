using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;
using RestSharp;

namespace Crossroads.Service.Finance.Services.PaymentProcessor
{
    public class PushpayPaymentProcessorService : IPaymentProcessorService
    {
        private readonly IRestClient _restClient;
        private const int PageSize = 25;
        private const int RequestsPerSecond = 10;
        private const int RequestsPerMinute = 60;

        public PushpayPaymentProcessorService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        // change to return a list of payment processer charge dtos - this is calling out to stripe to get the
        // transactions associated with that deposit in stripe

        // call out to the settlement/{settlementKey}/payments endpoint
        public PaymentsDto GetChargesForTransfer(string settlementKey)
        {
            var donationDtos = new List<DonationDto>();
            // add a loop here so that we continue to call it, until we don't get a next page (there is a next
            // page link in the pushpay api)

            // we will need to put a delay in here to avoid hitting the rate limit





            var url = $"settlement/{settlementKey}/payments";
            var request = new RestRequest(url, Method.GET);
            //request.AddParameter("count", _maxQueryResultsPerPage);

            var paymentsDto = new PaymentsDto();

            var response = _restClient.Execute<PaymentsDto>(request);

            // once we get the first data back from the response, we determine a delay based on the expected number of calls
            // we need to make

            // if response.totalRecords / 25 < 10, delay = 0
            // else if response.totalRecords / 25 < 60 = 150ms
            // else if response.totalRecords /25 > 60 = 1000ms

            int totalRecords = 200;

            for (int i = 0; i < totalRecords; i++)
            {
                Thread.Sleep(150);

                // call and parse next load
            }




            return response.Data;

            //StripeCharges nextPage;
            //do
            //{
            //    var response = _stripeRestClient.Execute<StripeCharges>(request);
            //    CheckStripeResponse("Could not query transactions", response);

            //    nextPage = response.Data;
            //    charges.AddRange(nextPage.Data.Select(charge => charge));

            //    request = new RestRequest(url, Method.GET);
            //    request.AddParameter("count", _maxQueryResultsPerPage);
            //    request.AddParameter("starting_after", charges.Last().Id);
            //} while (nextPage.HasMore);


            //var charges = paymentsDto.

            ////get the metadata for all of the charges
            //for (int i = charges.Count - 1; i >= 0; --i)
            //{
            //    StripeCharge charge = charges[i];

            //    // don't let a failure to retrieve data for one charge stop the entire batch
            //    try
            //    {
            //        if (charge.Type == "payment" || charge.Type == "charge")
            //        {
            //            var singlecharge = GetCharge(charge.Id);
            //            charge.Metadata = singlecharge.Metadata;
            //        }
            //        else if (charge.Type == "payment_refund") //its a bank account refund
            //        {
            //            var singlerefund = GetRefund(charge.Id);
            //            charge.Metadata = singlerefund.Charge.Metadata;
            //        }
            //        else // if charge.Type == "refund", it's a credit card charge refund
            //        {
            //            var singlerefund = GetChargeRefund(charge.Id);
            //            charge.Metadata = singlerefund.Data[0].Charge.Metadata;
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        // remove from the batch and keep going; the batch will be out of balance, but thats Ok
            //        _logger.Error($"GetChargesForTransfer error retrieving metadata for {charge.Type} {charge.Id}", e);
            //        charges.RemoveAt(i);
            //    }
            //}

            //return (charges);
        }
    }
}
