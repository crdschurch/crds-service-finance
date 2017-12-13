using System.Collections.Generic;
using Crossroads.Service.Finance.Models;

namespace Mock
{
    public class PaymentDtoMock
    {
        public static PaymentDto Create() =>
            new PaymentDto
            {
                TransactionId = "0abc",
                Amount = new AmountDto { Amount = "10" }
            };

        public static List<PaymentDto> CreateList() =>
            new List<PaymentDto>
            {
                new PaymentDto
                {
                    TransactionId = "1a",
                    Amount = new AmountDto { Amount = "20" }
                },
                new PaymentDto
                {
                    TransactionId = "2b",
                    Amount = new AmountDto { Amount = "30" }
                },
                new PaymentDto
                {
                    TransactionId = "3c",
                    Amount = new AmountDto { Amount = "40" }
                },
            };

        public static PaymentDto CreateEmpty() => new PaymentDto { };
    }
}
