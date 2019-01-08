using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Web.Common.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Crossroads.Service.Finance.Middleware
{
    public class ImpersonationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IContactService _contactService;

        public ImpersonationMiddleware(RequestDelegate next, IContactService contactService)
        {
            _next = next;
            _contactService = contactService;
        }

        public async Task Invoke(HttpContext context)
        {
            // look for the impersonation header
            if (context.Request.Headers.ContainsKey("ImpersonateUserEmail") && !String.IsNullOrEmpty(context.Request.Headers["ImpersonateUserEmail"]))
            {
                try
                {
                    var impersonatedUserEmail = context.Request.Headers["ImpersonateUserEmail"];
                    var userContactId = _contactService.GetContactIdByEmailAddress(impersonatedUserEmail);
                    var impersonatedContactId = new KeyValuePair<String, StringValues>("ImpersonatedContactId", userContactId.ToString());
                    context.Request.Headers.Add(impersonatedContactId);

                    Console.WriteLine($"Impersonated user: {impersonatedUserEmail}");
                    await _next.Invoke(context);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }

    public static class ImpersonationMiddlewareExtensions
    {
        public static IApplicationBuilder UseImpersonationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ImpersonationMiddleware>();
        }
    }
}

