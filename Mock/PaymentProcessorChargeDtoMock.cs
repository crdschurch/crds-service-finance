using System.Collections.Generic;
using Crossroads.Service.Finance.Models;

namespace Mock
{
    public class PaymentProcessorChargeDtoMock
    {
        public static PaymentProcessorChargeDto Create() =>
            new PaymentProcessorChargeDto
            {
                TransactionId = "0abc",
                Amount = new AmountDto { Amount = "10" }
            };

        public static List<PaymentProcessorChargeDto> CreateList() =>
            new List<PaymentProcessorChargeDto>
            {
                new PaymentProcessorChargeDto
                {
                    TransactionId = "1a",
                    Amount = new AmountDto { Amount = "20" }
                },
                new PaymentProcessorChargeDto
                {
                    TransactionId = "2b",
                    Amount = new AmountDto { Amount = "30" }
                },
                new PaymentProcessorChargeDto
                {
                    TransactionId = "3c",
                    Amount = new AmountDto { Amount = "40" }
                },
            };

        public static PaymentProcessorChargeDto CreateEmpty() => new PaymentProcessorChargeDto { };
    }
}
