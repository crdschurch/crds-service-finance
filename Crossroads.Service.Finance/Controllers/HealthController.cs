using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Services.Health;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private IHealthService _healthService;

        public HealthController(IHealthService healthService)
        {
            _healthService = healthService;
        }

        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> GetHealth(int contactId)
        {
            try
            {
                if ((await _healthService.GetHangfireStatus()) == false)
                {
                    Console.WriteLine($"Hangfire Heartbeat Failed");
                    _logger.Error($"Hangfire Heartbeat Failed");
                    return StatusCode(500, "Hangfire Heartbeat Failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HealthController.Get Health: {ex.Message}");
                _logger.Error(ex,$"HealthController.Get Health: {ex.Message}");
                return StatusCode(500, "Hangfire Monitoring Failed");
            }

            return StatusCode(200, "OK");
        }
    }
}
