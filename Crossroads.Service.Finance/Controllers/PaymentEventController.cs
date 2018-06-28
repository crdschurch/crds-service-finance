using System;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class PaymentEventController : Controller
    {
        private readonly IPaymentEventService _paymentEventService;

        public PaymentEventController(IPaymentEventService paymentEventService)
        {
            _paymentEventService = paymentEventService;
        }

        [HttpPost("settlement")]
        public IActionResult ProcessPaymentEvent([FromBody] SettlementEventDto settlementEventDto)
        {
            try
            {
                _paymentEventService.CreateDeposit(settlementEventDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex);
            }
        }

        [HttpPost("anticipated")]
        public IActionResult CreateAnticipatedPayment([FromBody] PushpayAnticipatedPaymentDto anticipatedPaymentDto)
        {
            try
            {
                var result = _paymentEventService.CreateAnticipatedPayment(anticipatedPaymentDto);
                return StatusCode(201, result.Links.Pay.Href);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex);
            }
        }
    }
}
