using System;
using Crossroads.Service.Finance.Security;
using Crossroads.Web.Common.Security;
using Crossroads.Web.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Headers;

namespace Crossroads.Service.Finance.Security
{
    public abstract class MpAuthBaseController : ControllerBase
    {
        /// <summary>
        /// An MP Authentication Repository for use by subclasses.
        /// </summary>
        protected readonly IAuthenticationRepository AuthenticationRepository;
        private readonly IAuthTokenExpiryService _authTokenExpiryService;

        /// <summary>
        /// Construct a new MpAuth with the given authenticationRepository
        /// </summary>
        /// <param name="authTokenExpiryService">an AuthTokenExpiryService instance</param>
        /// <param name="authenticationRepository">an Authentication Repository instance</param>
        protected MpAuthBaseController(IAuthenticationRepository authenticationRepository,
            IAuthTokenExpiryService authTokenExpiryService)
        {
            _authTokenExpiryService = authTokenExpiryService;
            AuthenticationRepository = authenticationRepository;
        }

        /// <summary>
        /// Ensure that a user is authenticated before executing the given lambda expression.  The expression will
        /// have a reference to the user's authentication token (the value of the "Authorization" cookie).  If
        /// the user is not authenticated, an UnauthorizedResult will be returned.
        /// </summary>
        /// <param name="doIt">A lambda expression to execute if the user is authenticated</param>
        /// <returns>An IHttpActionResult from the "doIt" expression, or UnauthorizedResult if the user is not authenticated.</returns>
        protected IActionResult Authorized(Func<string, IActionResult> doIt)
        {
            return Authorized(doIt, () => Unauthorized());
        }

        /// <summary>
        /// Execute the lambda expression "actionWhenAuthorized" if the user is authenticated, or execute the expression
        /// "actionWhenNotAuthorized" if the user is not authenticated.  If authenticated, the "actionWhenAuthorized"
        /// expression will have a reference to the user's authentication token (the value of the "Authorization" cookie).
        /// </summary>
        /// <param name="actionWhenAuthorized">A lambda expression to execute if the user is authenticated</param>
        /// <param name="actionWhenNotAuthorized">A lambda expression to execute if the user is NOT authenticated</param>
        /// <returns>An IHttpActionResult from the lambda expression that was executed.</returns>
        protected IActionResult Authorized(Func<string, IActionResult> actionWhenAuthorized, Func<IActionResult> actionWhenNotAuthorized)
        {
            try
            {
                var refreshTokenHeader = Request.Headers.Keys.Contains(HttpAuthResult.RefreshTokenHeaderName)
                    ? Request.Headers[HttpAuthResult.RefreshTokenHeaderName].ToString()
                    : null;

                bool isRefreshTokenPresent = refreshTokenHeader != null;
                bool isAuthTokenCloseToExpiry = _authTokenExpiryService.IsAuthTokenCloseToExpiry(Request.Headers);
                bool shouldGetNewAuthToken = isAuthTokenCloseToExpiry && isRefreshTokenPresent;

                if (shouldGetNewAuthToken)
                {
                    var authData = AuthenticationRepository.RefreshToken(refreshTokenHeader);
                    if (authData != null)
                    {
                        var authToken = authData.AccessToken;
                        var refreshToken = authData.RefreshToken;
                        var result = new HttpAuthResult(actionWhenAuthorized(authToken), authToken, refreshToken);
                        return result;
                    }
                }

                var authorized = Request.Headers.Keys.Contains(HttpAuthResult.AuthorizationTokenHeaderName)
                    ? Request.Headers[HttpAuthResult.AuthorizationTokenHeaderName].ToString()
                    : null;
                if (!string.IsNullOrEmpty(authorized))
                {
                    return actionWhenAuthorized(authorized);
                }
                return actionWhenNotAuthorized();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return actionWhenNotAuthorized();
            }
        }
    }
}
