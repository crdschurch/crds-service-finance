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

        public DepositController(IDepositService depositService)
        {
            _depositService = depositService;
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
        [Description("Sync settlements from pushpay into MP, called via a SyncPushpaySettlements job at 1pm every day")]
        public IActionResult SyncSettlements()
        {
            try
            {
                var hostName = this.Request.Host.ToString();
                _depositService.SyncDeposits(hostName);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error("Error in SyncSettlements: " + ex.Message, ex);
                return StatusCode(400, ex);
            }
        }
    }
}
