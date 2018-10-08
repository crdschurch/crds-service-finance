using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        public HealthController()
        {
            
        }

        [HttpGet]
        [Route("status")]
        public IActionResult GetHealth(int contactId)
        {
            return StatusCode(200, "OK");
        }
    }
}
