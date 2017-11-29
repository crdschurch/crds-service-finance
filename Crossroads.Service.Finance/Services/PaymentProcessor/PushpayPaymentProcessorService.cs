using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services.PaymentProcessor
{
    public class PushpayPaymentProcessorService : IPaymentProcessorService
    {
        public List<StripeCharge> GetChargesForTransfer(string transferId)
        {
            var url = $"transfers/{transferId}/transactions";
            var request = new RestRequest(url, Method.GET);
            request.AddParameter("count", _maxQueryResultsPerPage);

            var charges = new List<StripeCharge>();
            StripeCharges nextPage;
            do
            {
                var response = _stripeRestClient.Execute<StripeCharges>(request);
                CheckStripeResponse("Could not query transactions", response);

                nextPage = response.Data;
                charges.AddRange(nextPage.Data.Select(charge => charge));

                request = new RestRequest(url, Method.GET);
                request.AddParameter("count", _maxQueryResultsPerPage);
                request.AddParameter("starting_after", charges.Last().Id);
            } while (nextPage.HasMore);

            //get the metadata for all of the charges
            for (int i = charges.Count - 1; i >= 0; --i)
            {
                StripeCharge charge = charges[i];

                // don't let a failure to retrieve data for one charge stop the entire batch
                try
                {
                    if (charge.Type == "payment" || charge.Type == "charge")
                    {
                        var singlecharge = GetCharge(charge.Id);
                        charge.Metadata = singlecharge.Metadata;
                    }
                    else if (charge.Type == "payment_refund") //its a bank account refund
                    {
                        var singlerefund = GetRefund(charge.Id);
                        charge.Metadata = singlerefund.Charge.Metadata;
                    }
                    else // if charge.Type == "refund", it's a credit card charge refund
                    {
                        var singlerefund = GetChargeRefund(charge.Id);
                        charge.Metadata = singlerefund.Data[0].Charge.Metadata;
                    }
                }
                catch (Exception e)
                {
                    // remove from the batch and keep going; the batch will be out of balance, but thats Ok
                    _logger.Error($"GetChargesForTransfer error retrieving metadata for {charge.Type} {charge.Id}", e);
                    charges.RemoveAt(i);
                }
            }

            return (charges);
        }
    }
}
