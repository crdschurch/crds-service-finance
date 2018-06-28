using System;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.Service.Finance.Controllers
{
    public class WebhookController : Controller
    {
        private readonly IPushpayService _pushpayService;

        public WebhookController(IPushpayService pushpayService)
        {
            _pushpayService = pushpayService;
        }

        [HttpPost]
        [ActionName("pushpay")]
        public IActionResult HandlePushpayWebhooks([FromBody] PushpayWebhook pushpayWebhook)
        {
            try
            {
                var pushpayEvent = pushpayWebhook.Events[0];
                switch (pushpayEvent.EventType)
                {
                    case "payment_created":
                    case "payment_status_changed":
                        _pushpayService.AddUpdateStatusJob(pushpayWebhook);
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