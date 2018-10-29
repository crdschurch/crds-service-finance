using System;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Utilities.Logging;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class WebhookController : Controller
    {
        private readonly IPushpayService _pushpayService;
        private readonly IDataLoggingService _dataLoggingService;

        public WebhookController(IPushpayService pushpayService, IDataLoggingService dataLoggingService)
        {
            _pushpayService = pushpayService;
            _dataLoggingService = dataLoggingService;
        }

        /// <summary>
        /// Handles the pushpay webhooks.
        /// </summary>
        /// <remarks>
        ///    Called by Pushpay when a new donation is created, or a recurring gift is created/updated
        /// </remarks>
        /// <param name="pushpayWebhook">Pushpay webhook.</param>
        [HttpPost("pushpay")]
        [ProducesResponseType(200)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult HandlePushpayWebhooks([FromBody] PushpayWebhook pushpayWebhook)
        {
            try
            {
                Console.WriteLine("⚡️⚡️⚡️⚡️⚡️⚡️⚡️⚡ Incoming webhook ⚡️⚡️⚡️⚡️⚡️⚡️⚡️⚡️");
                Console.WriteLine(JsonConvert.SerializeObject(pushpayWebhook, Formatting.Indented));
                _pushpayService.SaveWebhookData(pushpayWebhook);

                var pushpayEvent = pushpayWebhook.Events[0];

                var logEventEntry = new LogEventEntry(LogEventType.incomingPushpayWebhook);
                logEventEntry.Push("webhookType", pushpayEvent.EventType);
                _dataLoggingService.LogDataEvent(logEventEntry);

                switch (pushpayEvent.EventType)
                {
                    case "payment_created":
                    case "payment_status_changed":
                        // add incoming timestamp so that we can reprocess job for a
                        //   certain amount of time
                        pushpayWebhook.IncomingTimeUtc = DateTime.UtcNow;
                        _pushpayService.UpdateDonationDetails(pushpayWebhook);
                        return Ok();
                    case "recurring_payment_changed":
                        var updatedGift = _pushpayService.UpdateRecurringGift(pushpayWebhook);
                        return StatusCode(200, updatedGift);
                    case "recurring_payment_created":
                        var newGift = _pushpayService.CreateRecurringGift(pushpayWebhook);
                        return StatusCode(201, newGift);
                    default:
                        return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Webhook error: {ex.Message}");
                return StatusCode(400, ex);
            }
        }
    }
}