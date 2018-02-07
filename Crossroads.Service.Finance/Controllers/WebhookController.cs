using System;
using System.Threading;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Hangfire;
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
            try
            {
                _pushpayService.AddUpdateStatusJob(pushpayWebhook);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex);
            }
        }

        [HttpPost]
        [Route("payment/created")]
        public IActionResult PaymentCreated([FromBody] PushpayWebhook pushpayWebhook)
        {
            try
            {
                _pushpayService.AddUpdateStatusJob(pushpayWebhook);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex);
            }
        }

        [HttpPost]
        [Route("recurring-gift/created")]
        public IActionResult RecurringGiftCreated([FromBody] PushpayWebhook pushpayWebhook)
        {
            try
            {
                var gift = _pushpayService.CreateRecurringGift(pushpayWebhook);
                return StatusCode(201, gift);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex);
            }
        }

        [HttpPost]
        [Route("recurring-gift/changed")]
        public IActionResult RecurringGiftChanged([FromBody] PushpayWebhook pushpayWebhook)
        {
            try
            {
                var gift = _pushpayService.UpdateRecurringGift(pushpayWebhook);
                return StatusCode(200, gift);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex);
            }
        }
    }
}
