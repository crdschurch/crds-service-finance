using System;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/webhook")]
    public class WebhookController : Controller
    {
        private readonly IPushpayService _pushpayService;

        public WebhookController(IPushpayService pushpayService)
        {
            _pushpayService = pushpayService;
        }

        [HttpPost]
        [Route("payment/status")]
        public IActionResult PaymentStatusUpdate([FromBody] PushpayWebhook pushpayWebhook)
        {
            return Ok(_pushpayService.UpdateDonationStatusFromPushpay(pushpayWebhook));
        }

        [HttpPost]
        [Route("payment/created")]
        public IActionResult PaymentCreated([FromBody] PushpayWebhook pushpayWebhook)
        {
            return Ok(_pushpayService.UpdateDonationStatusFromPushpay(pushpayWebhook));
        }
    }
}
