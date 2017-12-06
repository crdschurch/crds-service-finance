using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Pushpay;
using Pushpay.Client;

namespace Crossroads.Service.Finance.Services
{
    public class PushpayService : IPushpayService
    {
        private readonly IPushpayClient _pushpayClient;

        public PushpayService(IPushpayClient pushpayClient)
        {
            _pushpayClient = pushpayClient;
        }

        public PaymentsDto GetChargesForTransfer(string settlementKey)
        {
            var result = _pushpayClient.GetPushpayDonations(settlementKey);

            // TODO: need to iterate over result and convert objects to a payment dto
            return null;
        }

        // TODO replace
        public Boolean DoStuff()
        {
            _pushpayClient.DoStuff();
            return true;
        }

    }
}
