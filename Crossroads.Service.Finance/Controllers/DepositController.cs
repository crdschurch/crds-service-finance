﻿using Crossroads.Service.Finance.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class DepositController : Controller
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

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
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SyncSettlements()
        {
            try
            {
                var deposits = await _depositService.SyncDeposits();
                if (deposits == null || deposits.Count == 0)
                {
                    _logger.Info("No deposits to sync");
                    return NoContent();
                }
                foreach (var deposit in deposits)
                {
                    _paymentEventService.CreateDeposit(deposit);
                    Thread.Sleep(5000);
                }

                _logger.Info($"SyncSettlements processed {deposits.Count} deposits");

                return Ok(new {created = deposits.Count});
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in SyncSettlements: {ex.Message}, {ex}");

                return StatusCode(500);
            }
        }
    }
}
