using System;
using Crossroads.Service.Finance.Services.Interfaces;
using Pushpay;

namespace Crossroads.Service.Finance.Services.Pushpay
{
    public class PushpayService : IPushpayService
    {
        readonly PushpayClient _pushpayClient;

        public PushpayService()
        {
            _pushpayClient = new PushpayClient();
        }

        // TODO replace
        public Boolean DoStuff()
        {
            _pushpayClient.DoStuff();
            return true;
        }

    }
}
