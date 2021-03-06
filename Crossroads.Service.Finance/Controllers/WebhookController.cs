﻿using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class WebhookController : Controller
    {
        private readonly IPushpayService _pushpayService;
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public WebhookController(IPushpayService pushpayService)
        {
            _pushpayService = pushpayService;
        }

        /// <summary>
        /// Handles the pushpay webhooks.
        /// </summary>
        /// <remarks>
        ///    Called by Pushpay when a new donation is created, or a recurring gift is created/updated
        /// </remarks>
        /// <param name="pushpayWebhook">Pushpay webhook.</param>
        /// <param name="congregationId"></param>
        [HttpPost("pushpay/{congregationId?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public IActionResult HandlePushpayWebhooks([FromBody] PushpayWebhook pushpayWebhook, int? congregationId = null)
        {
            try
            {
                pushpayWebhook.CongregationId = congregationId;

                _logger.Info("⚡️⚡️⚡️⚡️⚡️⚡️⚡️⚡ Incoming webhook ⚡️⚡️⚡️⚡️⚡️⚡️⚡️⚡️");
                _logger.Info(JsonConvert.SerializeObject(pushpayWebhook, Formatting.Indented));
                _pushpayService.SaveWebhookData(pushpayWebhook);

                var pushpayEvent = pushpayWebhook.Events[0];

                switch (pushpayEvent.EventType)
                {
                    case "recurring_payment_changed":
                        var updatedGift = _pushpayService.UpdateRecurringGift(pushpayWebhook, congregationId);
                        return StatusCode(200);
                    case "recurring_payment_created":
                        var newGift = _pushpayService.CreateRecurringGift(pushpayWebhook, congregationId);
                        return StatusCode(201);
                    default:
                        return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error WebhookController.HandlePushpayWebhooks: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}