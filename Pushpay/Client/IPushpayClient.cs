using System;
using System.Collections.Generic;
using System.Text;
using Pushpay.Models;

namespace Pushpay.Client
{
    public interface IPushpayClient
    {
        IObservable<OAuth2TokenResponse> GetOAuthToken();
        PushpayPaymentsDto GetPushpayDonations(string settlementKey);
        IObservable<bool> DoStuff();
    }
}
