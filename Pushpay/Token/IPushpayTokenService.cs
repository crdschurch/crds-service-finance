using System;
using System.Threading.Tasks;
using Pushpay.Models;

namespace Pushpay.Token
{
    public interface IPushpayTokenService {
        Task<OAuth2TokenResponse> GetOAuthToken(string scope = "read");
    }
}
