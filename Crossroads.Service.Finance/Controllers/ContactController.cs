using Crossroads.Web.Common.Security;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using Crossroads.Service.Finance.Security;
using Crossroads.Web.Common.Services;
using Crossroads.Service.Finance.Interfaces;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class ContactController : MpAuthBaseController
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
            return Authorized(token =>
            {
                try
                {
                    //return Ok(_contactService.GetContact(contactId)); // Don't delete
                    var contact = _contactService.GetContact(contactId);

                    if (contact.ContactId == 0)
                    {
                        _logger.Error("Default contact id of 0 for token: " + token);
                    }

                    return Ok(contact);
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
            try
            {
                return Ok(_contactService.GetBySessionId(sessionId));
            }
            catch (Exception ex)
            {
                _logger.Error("Error in GetContactBySessionId: " + ex.Message, ex);
                return StatusCode(401, ex);
            }
        }
    }
}
