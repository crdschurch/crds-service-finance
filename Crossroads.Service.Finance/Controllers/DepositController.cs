using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Utilities.Logging;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class DepositController : Controller
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDepositService _depositService;
        private readonly IPaymentEventService _paymentEventService;
        private readonly IDataLoggingService _dataLoggingService;

        public DepositController(IDepositService depositService, IPaymentEventService paymentEventService,
            IDataLoggingService dataLoggingService)
        {
            _depositService = depositService;
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
        public async Task<IActionResult> SyncSettlements()
        {
            try
            {
                var deposits = await _depositService.SyncDeposits();
                if (deposits == null || deposits.Count == 0)
                {
                    Console.WriteLine($"No deposits to sync");

                    var noDepositsToSyncEntry = new LogEventEntry(LogEventType.noDepositsToSync);
                    noDepositsToSyncEntry.Push("syncDate", DateTime.Now.ToShortDateString());
                    _dataLoggingService.LogDataEvent(noDepositsToSyncEntry);

                    return NoContent();
                }
                foreach (var deposit in deposits)
                {
                    //var serializedDataTask = new Task<string>(() => SerializeJournalEntryStages(velosioJournalEntryBatch));
                    _paymentEventService.CreateDeposit(deposit);
                }
                Console.WriteLine($"SyncSettlements created {deposits.Count} deposits");

                var logEventEntry = new LogEventEntry(LogEventType.depositsCreatedCount);
                logEventEntry.Push("depositsCreatedCount", deposits.Count);
                _dataLoggingService.LogDataEvent(logEventEntry);

                return Ok(new {created = deposits.Count});
            }
            catch (Exception ex)
            {
                _logger.Error("Error in SyncSettlements: " + ex.Message, ex);
                return StatusCode(400, ex);
            }
        }
    }
}
