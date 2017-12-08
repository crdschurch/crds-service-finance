using System;
using Pushpay.Models;

namespace Pushpay.Token
{
    public interface IPushpayTokenService {
        IObservable<OAuth2TokenResponse> GetOAuthToken(string scope = "read");
    }
}
