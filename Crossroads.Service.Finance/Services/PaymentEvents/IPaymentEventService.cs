using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IPaymentEventService
    {
        void CreateDeposit(SettlementEventDto settlementEventDto);
    }
}
