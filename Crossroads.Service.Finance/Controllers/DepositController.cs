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
                _depositService.SyncDeposits();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex);
            }
        }

        [HttpGet]
        //[Route("sync")]
        public IActionResult GetSettlements()
        {
            try
            {
                // TODO: Change hardcoded date time values to live values passed in via the service
                var startDate = DateTime.Now.AddDays(-14);
                var endDate = DateTime.Now;

                var result = _depositService.GetDepositsForSync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex);
            }
        }
    }
}
