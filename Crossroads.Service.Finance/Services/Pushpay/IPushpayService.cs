using Crossroads.Service.Finance.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IPushpayService
    {
        PaymentsDto GetChargesForTransfer(string settlementKey);
        PushpayPaymentDto GetPayment(PushpayWebhook webhook);
        PaymentDto UpdatePayment(PaymentDto payment);
    }
}
