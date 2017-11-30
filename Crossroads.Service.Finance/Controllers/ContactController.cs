using System;
using System.Linq;
using Crossroads.Service.Finance.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Pushpay;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class ContactController : Controller
    {
        readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        [Route("hello")]
        public IActionResult HelloWorld()
        {
            return Ok("hello world");
        }

        [HttpGet]
        [Route("auth")]
        public IActionResult Auth()
        {
            try 
            {
                Client.GetOAuthToken();
                return Ok();    
            } 
            catch (Exception e)
            {
                return BadRequest();
            }

        }

        [HttpGet]
        [Route("{contactId}")]
        public IActionResult GetContact(int contactId)
        {
            return Ok(_contactService.GetContact(contactId));
        }

    }
}
