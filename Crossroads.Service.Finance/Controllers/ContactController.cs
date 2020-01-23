﻿using Crossroads.Web.Common.Security;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Crossroads.Web.Common.Services;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Web.Auth.Controllers;
using Crossroads.Web.Auth.Models;
using Crossroads.Web.Common.Auth.Helpers;

namespace Crossroads.Service.Finance.Controllers
{
    [RequiresAuthorization]
    [Route("api/[controller]")]
    public class ContactController : AuthBaseController
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        readonly IContactService _contactService;

        public ContactController(IAuthTokenExpiryService authTokenExpiryService,
            IContactService contactService,
            IAuthenticationRepository authenticationRepository)
            : base(authenticationRepository, authTokenExpiryService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        [Route("{contactId}")]
        public async Task<IActionResult> GetContact(int contactId)
        {
            try
            {
                return Ok(await _contactService.GetContact(contactId));
            }
            catch (Exception ex)
            {
                _logger.Error("Error in Contact: " + ex.Message, ex);
                return StatusCode(400, ex);
            }
        }

        [HttpGet]
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
                _logger.Error("Error in Contact: " + ex.Message, ex);
                return StatusCode(400, ex);
            }
        }

        [HttpGet]
        [Route("{contactId}/address")]
        public async Task<IActionResult> GetContactAddress(int contactId)
        {
            try
            {
                return Ok(await _contactService.GetContactAddressByContactId(contactId));
            }
            catch (Exception ex)
            {
                _logger.Error("Error in Contact: " + ex.Message, ex);
                return StatusCode(400, ex);
            }
        }
    }
}
