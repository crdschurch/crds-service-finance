using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services.Recurring;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class RecurringController : Controller
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IRecurringService _recurringService;
        private readonly IPaymentEventService _paymentEventService;

        public RecurringController(IRecurringService recurringService, IPaymentEventService paymentEventService)
        {
            _recurringService = recurringService;
            _paymentEventService = paymentEventService;
        }

        /// <summary>
        ///    Sync settlements from pushpay into MP
        /// </summary>
        /// <remarks>
        ///    Called via a SyncPushpaySettlements windows scheduled task at 1pm every day
        /// </remarks>
        [HttpPost("sync")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SyncRecurringGifts([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                await _recurringService.SyncRecurringGifts(startDate, endDate);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RecurringController.SyncRecurringGifts: {ex.Message}");
                _logger.Error(ex, $"Error in RecurringController.SyncRecurringGifts: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
