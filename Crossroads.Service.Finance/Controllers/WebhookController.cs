using System;
using Crossroads.Service.Finance.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/webhook")]
    public class WebhookController : Controller
    {
        [HttpPost]
        [Route("donation")]
        public IActionResult DonationStatusUpdate([FromBody] PushpayWebhook pushpayWebhook)
        {
            return Json(pushpayWebhook);
        }
    }
}
