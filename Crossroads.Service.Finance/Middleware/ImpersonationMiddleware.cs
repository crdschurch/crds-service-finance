using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Web.Common.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ProcessLogging.Models;
using ProcessLogging.Transfer;

namespace Crossroads.Service.Finance.Middleware
{
    public class ImpersonationMiddleware
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly RequestDelegate _next;
        private readonly IContactService _contactService;
        private readonly IProcessLogger _processLogger;

        public ImpersonationMiddleware(RequestDelegate next, IContactService contactService, IProcessLogger processLogger)
        {
            _next = next;
            _contactService = contactService;
            _processLogger = processLogger;
        }

        public async Task Invoke(HttpContext context)
        {
            // look for the impersonation header
            if (context.Request.Headers.ContainsKey("ImpersonateUserEmail") && !String.IsNullOrEmpty(context.Request.Headers["ImpersonateUserEmail"]))
            {
                try
                {
                    var impersonatedUserEmail = context.Request.Headers["ImpersonateUserEmail"];
                    var userContactId = await _contactService.GetContactIdByEmailAddress(impersonatedUserEmail);
                    var impersonatedContactId = new KeyValuePair<String, StringValues>("ImpersonatedContactId", userContactId.ToString());
                    context.Request.Headers.Add(impersonatedContactId);

                    //Console.WriteLine($"Impersonated user: {impersonatedUserEmail}");
                    //_logger.Info($"Impersonated user: {impersonatedUserEmail}");

                    var impersonatingUserMessage = new ProcessLogMessage(ProcessLogConstants.MessageType.impersonatingUser)
                    {
                        MessageData = $"Impersonating {userContactId}"
                    };
                    _processLogger.SaveProcessLogMessage(impersonatingUserMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ImpersonationMiddleware.Invoke: {ex.Message}");
                    _logger.Error(ex, $"Error in ImpersonationMiddleware.Invoke: {ex.Message}");
                    throw;
                }
            }

            await _next.Invoke(context);
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

