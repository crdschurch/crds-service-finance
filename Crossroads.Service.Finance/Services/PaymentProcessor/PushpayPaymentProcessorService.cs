using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;
using RestSharp;

namespace Crossroads.Service.Finance.Services.PaymentProcessor
{
    public class PushpayPaymentProcessorService : IPaymentProcessorService
    {
        private readonly IRestClient _restClient;

        public PushpayPaymentProcessorService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        // change to return a list of payment processer charge dtos - this is calling out to stripe to get the
        // transactions associated with that deposit in stripe

        // call out to the settlement/{settlementKey}/payments endpoint
        public PaymentsDto GetChargesForTransfer(string settlementKey)
        {
            var url = $"settlement/{settlementKey}/payments";
            var request = new RestRequest(url, Method.GET);
            //request.AddParameter("count", _maxQueryResultsPerPage);

            var paymentsDto = new PaymentsDto();

            var response = _restClient.Execute<PaymentsDto>(request);


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
