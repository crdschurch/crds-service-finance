using System;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Services.Slack;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly ISlackService _slackService;

        public HealthController(ISlackService slackService)
        {
	        _slackService = slackService;
        }

        [HttpGet]
        [Route("status")]
        public IActionResult GetHealth()
        {
            return StatusCode(200, "OK");
        }
    }
}
