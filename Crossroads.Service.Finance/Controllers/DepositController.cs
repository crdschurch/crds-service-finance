﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class DepositController : Controller
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDepositService _depositService;
        //private readonly ILogger<DepositController> _logger;

        public DepositController(IDepositService depositService)
        {
            _depositService = depositService;
            //_logger = logger;
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
                _logger.Debug("Getting active settlements");
                var result = _depositService.GetDepositsForSync(startdate, enddate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
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

        [HttpGet]
        [Route("pending-sync")]
        public IActionResult GetSettlementsPendingSync()
        {
            try
            {
                var result = _depositService.GetDepositsForPendingSync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex);
            }
        }
    }
}
