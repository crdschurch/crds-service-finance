using Crossroads.Service.Finance.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IPaymentEventService
    {
        void CreateDeposit(SettlementEventDto settlementEventDto);
        PushpayAnticipatedPaymentDto CreateAnticipatedPayment(PushpayAnticipatedPaymentDto anticipatedPaymentDto);
    }
}
