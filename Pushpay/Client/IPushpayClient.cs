using Crossroads.Service.Finance.Models;
using Pushpay.Models;

namespace Pushpay.Client
{
    public interface IPushpayClient
    {
        PushpayPaymentsDto GetPushpayDonations(string settlementKey);
        PushpayPaymentDto GetPayment(PushpayWebhook webhook);
    }
}
