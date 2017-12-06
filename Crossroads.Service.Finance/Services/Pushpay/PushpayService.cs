using System;
using Crossroads.Service.Finance.Interfaces;
using Pushpay;

namespace Crossroads.Service.Finance.Services
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
