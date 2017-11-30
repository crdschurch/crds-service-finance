using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Services.PaymentProcessor
{
    public interface IPaymentProcessorService
    {
        PaymentsDto GetChargesForTransfer(string settlementKey);
    }
}
