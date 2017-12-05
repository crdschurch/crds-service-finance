using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Interfaces
{
    public interface IPaymentProcessorService
    {
        PaymentsDto GetChargesForTransfer(string settlementKey);
    }
}
