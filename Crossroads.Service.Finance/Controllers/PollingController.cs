using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services.Recurring;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ProcessLogging.Models;
using ProcessLogging.Transfer;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class PollingController : Controller
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IPushpayService _pushpayService;
        private readonly IProcessLogger _processLogger;

        public PollingController(IPushpayService pushpayService,
                                IProcessLogger processLogger)
        {
            _pushpayService = pushpayService;
            _processLogger = processLogger;
        }

        /// <summary>
        ///    Polls for new payments roughly every five minutes
        /// </summary>
        [HttpPost("donations")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PollDonations()
        {
            try
            {
                _processLogger.SaveProcessLogMessage(new ProcessLogMessage(ProcessLogConstants.MessageType.jobStarting)
                {
                    MessageData = "Starting getting donation updates from PushPay."
                });
                await _pushpayService.PollDonations();
                _processLogger.SaveProcessLogMessage(new ProcessLogMessage(ProcessLogConstants.MessageType.jobDone)
                {
                    MessageData = "Finished getting updates for donations from PushPay."
                });
                return NoContent();
            }
            catch (Exception ex)
            {
                var error = $"Got error getting updates for donations from PushPay: {ex.Message}";
                _processLogger.SaveProcessLogMessage(new ProcessLogMessage(ProcessLogConstants.MessageType.jobErrored)
                {
                    MessageData = error
                });
                _logger.Error(error);
                return StatusCode(500);
            }
        }
    }
}