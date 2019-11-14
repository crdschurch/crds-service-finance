using Pushpay.Models;
using System.Threading.Tasks;

namespace Pushpay.Token
{
    public interface IPushpayTokenService {
        Task<OAuth2TokenResponse> GetOAuthToken(string scope = "read");
    }
}
