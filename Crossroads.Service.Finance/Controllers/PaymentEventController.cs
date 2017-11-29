using System;
using System.Linq;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Services.Interfaces;
using Crossroads.Service.Finance.Services.PaymentEvents;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.Service.Finance.Controllers
{
    [Route("api/[controller]")]
    public class PaymentEventController : Controller
    {
        readonly IPaymentEventService _paymentEventService;

        public PaymentEventController(IPaymentEventService paymentEventService)
        {
            _paymentEventService = paymentEventService;
        }

        [HttpGet]
        [Route("hello")]
        public IActionResult PaymentEventServiceHelloWorld()
        {
            return Ok("payment event service hello world");
        }

        [HttpGet]
        [Route("{contactId}")]
        public IActionResult ProcessPaymentEvent(SettlementEventDto settlementEventDto)
        {
            return Ok(_paymentEventService.CreateDeposit(settlementEventDto));
        }

    }
}
