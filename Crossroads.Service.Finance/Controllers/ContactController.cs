using Crossroads.Web.Common.Security;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Security;
using Crossroads.Web.Common.Services;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Web.Auth.Controllers;

namespace Crossroads.Service.Finance.Controllers
{
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
        public IActionResult GetContact(int contactId)
        {
            return Authorized(authDto =>
            {
                try
                {
                    return Ok(_contactService.GetContact(contactId));
                }
                catch (Exception ex)
                {
                    _logger.Error("Error in Contact: " + ex.Message, ex);
                    return StatusCode(400, ex);
                }
            });
        }

        [HttpGet]
        [Route("session")]
        public IActionResult GetContactBySessionId(string sessionId)
        {
            return Authorized(authDto =>
            {
                try
                {
                    return Ok(_contactService.GetContact(authDto.UserInfo.Mp.ContactId));
                }
                catch (Exception ex)
                {
                    _logger.Error("Error in Contact: " + ex.Message, ex);
                    return StatusCode(400, ex);
                }
            });
        }

        [HttpGet]
        [Route("{contactId}/address")]
        public IActionResult GetContactAddress(int contactId)
        {
            return Authorized(authDto =>
            {
                try
                {
                    return Ok(_contactService.GetContactAddressByContactId(contactId));
                }
                catch (Exception ex)
                {
                    _logger.Error("Error in Contact: " + ex.Message, ex);
                    return StatusCode(400, ex);
                }
            });
        }
    }
}
