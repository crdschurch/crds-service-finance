using System;
using Crossroads.Service.Finance.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/webhook")]
    public class WebhookController : Controller
    {
        [HttpPost]
        [Route("payment/status")]
        public IActionResult PaymentStatusUpdate([FromBody] PushpayWebhook pushpayWebhook)
        {
            return Json(pushpayWebhook);
        }

        [HttpPost]
        [Route("payment/created")]
        public IActionResult PaymentCreated([FromBody] PushpayWebhook pushpayWebhook)
        {
            return Json(pushpayWebhook);
        }

        [HttpPost]
        [Route("payment/anticipated/status")]
        public IActionResult AnticipatedPaymentStatusUpdate([FromBody] PushpayWebhook pushpayWebhook)
        {
            return Json(pushpayWebhook);
        }
    }
}
