using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services.Recurring;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Utilities.Logging;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class RecurringController : Controller
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRecurringService _recurringService;
        private readonly IPaymentEventService _paymentEventService;
        private readonly IDataLoggingService _dataLoggingService;

        public RecurringController(IRecurringService recurringService, IPaymentEventService paymentEventService,
            IDataLoggingService dataLoggingService)
        {
            _recurringService = recurringService;
            _paymentEventService = paymentEventService;
            _dataLoggingService = dataLoggingService;
        }

        /// <summary>
        ///    Sync settlements from pushpay into MP
        /// </summary>
        /// <remarks>
        ///    Called via a SyncPushpaySettlements windows scheduled task at 1pm every day
        /// </remarks>
        [HttpPost("sync")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SyncRecurringGifts([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                await _recurringService.SyncRecurringGifts(startDate, endDate);
                return Ok();
            }
            catch (Exception ex)
            {
                var syncedRecurringGiftsError = new LogEventEntry(LogEventType.syncRecurringGiftsError);
                syncedRecurringGiftsError.Push("errorInSyncRecurringGifts", ex.Message);
                _dataLoggingService.LogDataEvent(syncedRecurringGiftsError);
                Console.WriteLine("Error in sync recurring gifts: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                return StatusCode(400, ex);
            }
        }
    }
}
