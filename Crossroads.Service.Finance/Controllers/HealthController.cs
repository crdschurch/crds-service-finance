using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> GetHealth(int contactId)
        {
            return StatusCode(200, "OK");
        }
    }
}
