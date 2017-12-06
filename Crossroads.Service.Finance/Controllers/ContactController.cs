using System;
using System.Linq;
using Crossroads.Service.Finance.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Pushpay;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class ContactController : Controller
    {
        readonly IContactService _contactService;
        readonly IPushpayService _pushpayService;

        public ContactController(IContactService contactService, IPushpayService pushpayService)
        {
            _contactService = contactService;
            _pushpayService = pushpayService;
        }

        [HttpGet]
        [Route("hello")]
        public IActionResult HelloWorld()
        {
            return Ok("hello world");
        }

        // TODO remove or replace
        [HttpGet]
        [Route("auth")]
        public IActionResult Auth()
        {
            try 
            {
                _pushpayService.DoStuff();
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
