using Crossroads.Service.Finance.Interfaces;
using Crossroads.Web.Auth.Controllers;
using Crossroads.Web.Auth.Models;
using Crossroads.Web.Common.Auth.Helpers;
using Crossroads.Web.Common.Security;
using Crossroads.Web.Common.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Controllers
{
    [RequiresAuthorization]
    [Route("api/[controller]")]
    public class ContactController : AuthBaseController
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        readonly IContactService _contactService;

        public ContactController(IAuthTokenExpiryService authTokenExpiryService,
            IContactService contactService,
            IAuthenticationRepository authenticationRepository)
            : base(authenticationRepository, authTokenExpiryService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Route("{contactId}")]
        public async Task<IActionResult> GetContact(int contactId)
        {
            try
            {
                return Ok(await _contactService.GetContact(contactId));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ContactController:GetContact for contactId={contactId}: {ex.Message}");
                _logger.Error(ex, $"Error in ContactController:GetContact for contactId={contactId}: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Route("session")]
        public async Task<IActionResult> GetContactBySessionId(string sessionId)
        {
            var authDto = (AuthDTO)HttpContext.Items["authDto"];

            try
            {
                return Ok( await _contactService.GetContact(authDto.UserInfo.Mp.ContactId));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ContactController:GetContactBySessionId for contactId={authDto.UserInfo.Mp.ContactId}: {ex.Message}");
                _logger.Error(ex, $"Error in ContactController:GetContactBySessionId for contactId={authDto.UserInfo.Mp.ContactId}: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Route("{contactId}/address")]
        public async Task<IActionResult> GetContactAddress(int contactId)
        {
            try
            {
                return Ok(await _contactService.GetContactAddressByContactId(contactId));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ContactController:GetContactAddress for contactId={contactId}: {ex.Message}");
                _logger.Error(ex, $"Error in ContactController:GetContactAddress for contactId={contactId}: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
