using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class DepositController : Controller
    {
        private readonly IDepositService _depositService;

        public DepositController(IDepositService depositService)
        {
            _depositService = depositService;
        }

        [HttpPost]
        [Route("sync")]
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
                return StatusCode(400, ex);
            }
        }

        [HttpGet]
        [Route("active")]
        public IActionResult GetActiveSettlements([FromQuery] DateTime startdate, [FromQuery] DateTime enddate)
        {
            try
            {
                var result = _depositService.GetDepositsForSync(startdate, enddate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex);
            }
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllSettlements([FromQuery] DateTime startdate, [FromQuery] DateTime enddate)
        {
            try
            {
                var result = _depositService.GetDepositsForSyncRaw(startdate, enddate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex);
            }
        }
    }
}
