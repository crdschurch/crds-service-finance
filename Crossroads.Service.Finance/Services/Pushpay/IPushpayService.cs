using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IPushpayService
    {
        Boolean DoStuff();
        PaymentsDto GetChargesForTransfer(string settlementKey);
    }
}
