using System;
using System.ComponentModel;
using System.Reflection;
using Crossroads.Service.Finance.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.Service.Finance.Controllers
{
    public class DepositController : Controller
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDepositService _depositService;

        public DepositController(IDepositService depositService)
        {
            _depositService = depositService;
        }

        [HttpPost]
        [ActionName("sync")]
        [Description("Sync settlements")]
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

        [HttpGet]
        [ActionName("active")]
        public IActionResult GetActiveSettlements([FromQuery] DateTime startdate, [FromQuery] DateTime enddate)
        {
            try
            {
                var result = _depositService.GetDepositsForSync(startdate, enddate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Error in GetActiveSettlements: " + ex.Message, ex);
                return StatusCode(400, ex);
            }
        }

        [HttpGet]
        [ActionName("all")]
        public IActionResult GetAllSettlements([FromQuery] DateTime startdate, [FromQuery] DateTime enddate)
        {
            try
            {
                var result = _depositService.GetDepositsForSyncRaw(startdate, enddate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Error in GetAllSettlements: " + ex.Message, ex);
                return StatusCode(400, ex);
            }
        }

        [HttpGet]
        [ActionName("pending-sync")]
        public IActionResult GetSettlementsPendingSync()
        {
            try
            {
                var result = _depositService.GetDepositsForPendingSync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Error in GetSettlementsPendingSync: " + ex.Message, ex);
                return StatusCode(400, ex);
            }
        }
    }
}
