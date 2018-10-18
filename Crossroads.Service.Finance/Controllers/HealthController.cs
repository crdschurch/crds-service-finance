using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using Crossroads.Service.Finance.Services.Health;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private IHealthService _healthService;

        public HealthController(IHealthService healthService)
        {
            _healthService = healthService;
        }

        [HttpGet]
        [Route("status")]
        public IActionResult GetHealth(int contactId)
        {
            try
            {
                if (_healthService.GetHangfireStatus() == false)
                {
                    return StatusCode(500, "Hangfire Heartbeat Failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hangfire Monitoring Failed");
            }

            return StatusCode(200, "OK");
        }
    }
}
