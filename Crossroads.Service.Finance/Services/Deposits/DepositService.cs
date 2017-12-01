using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;

namespace Crossroads.Service.Finance.Services.Deposits
{
    public class DepositService
    {
        public DepositDto CreateDeposit(SettlementEventDto settlementEventDto, string depositName)
        {
            var depositDto = new DepositDto
            {
                // Account number must be non-null, and non-empty; using a single space to fulfill this requirement
                AccountNumber = " ",
                BatchCount = 1,
                DepositDateTime = DateTime.Now,
                DepositName = depositName,
                // This is the amount from Stripe - will show out of balance if does not match batch total above

                // TODO: On stripe code, this was adding up the fees and the amount. If we do not have to worry about fees, this is irrelevant
                DepositTotalAmount = Decimal.Parse(settlementEventDto.TotalAmount.Amount),
                //ProcessorFeeTotal = stripeTotalFees / Constants.StripeDecimalConversionValue, // TODO: Verify if this needed or not
                DepositAmount = Decimal.Parse(settlementEventDto.TotalAmount.Amount),
                Exported = false,
                Notes = null,
                ProcessorTransferId = settlementEventDto.Key
            };

            return depositDto;
        }

        public DepositDto SaveDeposit(DepositDto depositDto)
        {
            // TODO: Implement this
        }
    }
}
