using Crossroads.Service.Finance.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using ProcessLogging.Models;
using ProcessLogging.Transfer;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class DepositController : Controller
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IDepositService _depositService;
        private readonly IPaymentEventService _paymentEventService;
        private readonly IProcessLogger _processLogger;

        public DepositController(IDepositService depositService, IPaymentEventService paymentEventService, IProcessLogger processLogger)
        {
            _depositService = depositService;
            _paymentEventService = paymentEventService;
            _processLogger = processLogger;
        }

        /// <summary>
        ///    Sync settlements from pushpay into MP
        /// </summary>
        /// <remarks>
        ///    Called via a SyncPushpaySettlements windows scheduled task at 1pm every day
        /// </remarks>
        [HttpPost("sync")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SyncSettlements()
        {
            try
            {
                var deposits = await _depositService.SyncDeposits();
                if (deposits == null || deposits.Count == 0)
                {
                    //Console.WriteLine("No deposits to sync");
                    //_logger.Info("No deposits to sync");
                    var noDepositsToSyncMessage = new ProcessLogMessage(ProcessLogConstants.MessageType.noDepositsToSync)
                    {
                        MessageData = $"No deposits to sync."
                    };
                    _processLogger.SaveProcessLogMessage(noDepositsToSyncMessage);

                    return NoContent();
                }
                foreach (var deposit in deposits)
                {
                    _paymentEventService.CreateDeposit(deposit);
                    Thread.Sleep(5000);
                }

                //Console.WriteLine($"SyncSettlements processed {deposits.Count} deposits");
                //_logger.Info($"SyncSettlements processed {deposits.Count} deposits");

                var depositsProcessedMessage = new ProcessLogMessage(ProcessLogConstants.MessageType.settlementsProcessed)
                {
                    MessageData = $"{deposits.Count} settlements were processed."
                };
                _processLogger.SaveProcessLogMessage(depositsProcessedMessage);

                return Ok(new {created = deposits.Count});
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SyncSettlements: {ex.Message}, {ex}");
                _logger.Error($"Error in SyncSettlements: {ex.Message}, {ex}");

                return StatusCode(500);
            }
        }
    }
}
