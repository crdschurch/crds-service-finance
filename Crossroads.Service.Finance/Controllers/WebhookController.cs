using System;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class WebhookController : Controller
    {
        private readonly IPushpayService _pushpayService;

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
        [HttpPost("pushpay")]
        [ProducesResponseType(200)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult HandlePushpayWebhooks([FromBody] PushpayWebhook pushpayWebhook)
        {
            //NewRelic.Api.Agent.NewRelic.AddCustomParameter("webhookPayload", 
            //    JsonConvert.SerializeObject(pushpayWebhook, Formatting.Indented));

            //NewRelic.Api.Agent.NewRelic.

            //NewRelic.Api.Agent.NewRelic.AddCustomParameter("webhookPayload",
            //    JsonConvert.SerializeObject(pushpayWebhook, Formatting.Indented));

            try
            {
                Console.WriteLine("⚡️⚡️⚡️⚡️⚡️⚡️⚡️⚡ Incoming webhook ⚡️⚡️⚡️⚡️⚡️⚡️⚡️⚡️");
                Console.WriteLine(JsonConvert.SerializeObject(pushpayWebhook, Formatting.Indented));
                var pushpayEvent = pushpayWebhook.Events[0];

                switch (pushpayEvent.EventType)
                {
                    case "payment_created":
                    case "payment_status_changed":
                        _pushpayService.AddUpdateDonationDetailsJob(pushpayWebhook);
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
                return StatusCode(400, ex);
            }
        }
    }
}