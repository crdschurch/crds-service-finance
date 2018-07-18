using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.Service.Finance.Security
{
    public class HttpAuthResult : IActionResult
    {
        private readonly string _token;
        private readonly string _refreshToken;
        private readonly IActionResult _result;

        public const string AuthorizationTokenHeaderName = "Authorization";
        public const string RefreshTokenHeaderName = "RefreshToken";

        public HttpAuthResult(IActionResult result, string token, string refreshToken)
        {
            _result = result;
            _token = token;
            _refreshToken = refreshToken;
        }

        public async Task ExecuteResultAsync(ActionContext ctx)
        {
            ctx.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", new[] { AuthorizationTokenHeaderName, RefreshTokenHeaderName });
            ctx.HttpContext.Response.Headers.Add(AuthorizationTokenHeaderName, _token);
            ctx.HttpContext.Response.Headers.Add(RefreshTokenHeaderName, _refreshToken);
            await _result.ExecuteResultAsync(ctx);
        }
    }
}
