using System;
using System.ComponentModel;
using System.Reflection;
using Crossroads.Service.Finance.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class DepositController : Controller
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDepositService _depositService;
        private readonly IPaymentEventService _paymentEventService;

        public DepositController(IDepositService depositService, IPaymentEventService paymentEventService)
        {
            _depositService = depositService;
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
        [ProducesResponseType(400)]
        public IActionResult SyncSettlements()
        {
            try
            {
                var deposits = _depositService.SyncDeposits();
                foreach (var deposit in deposits)
                {
                    _paymentEventService.CreateDeposit(deposit);
                }
                _logger.Info($"SyncSettlements created ${deposits.Count} deposits");
                return Ok(new { created =  deposits.Count });
            }
            catch (Exception ex)
            {
                _logger.Error("Error in SyncSettlements: " + ex.Message, ex);
                return StatusCode(400, ex);
            }
        }
    }
}
