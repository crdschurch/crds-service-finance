using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services.Recurring;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class PollingController : Controller
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IPushpayService _pushpayService;

        public PollingController(IPushpayService pushpayService)
        {
            _pushpayService = pushpayService;
        }

        /// <summary>
        ///    Sync settlements from pushpay into MP
        /// </summary>
        /// <remarks>
        ///    Called via a SyncPushpaySettlements windows scheduled task at 1pm every day
        /// </remarks>
        [HttpPost("donations")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PollDonations([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                _pushpayService.PollDonations();
                return Ok();
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error RecurringController.SyncRecurringGifts: {ex.Message}");
                //_logger.Error(ex, $"Error in RecurringController.SyncRecurringGifts: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}