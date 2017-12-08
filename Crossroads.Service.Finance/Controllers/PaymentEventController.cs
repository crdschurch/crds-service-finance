using System;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [Route("settlement")]
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
    }
}
