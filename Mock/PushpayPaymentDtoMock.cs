using System.Collections.Generic;
using Pushpay.Models;

namespace Mock
{
    public class PushpayPaymentDtoMock
    {
        public static PushpayPaymentDto CreateSuccess() =>
            new PushpayPaymentDto
            {
                Status = "Success",
                PaymentMethodType = "ACH",
                Account = new PushpayAccount {},
                Payer = new PushpayPayer {},
                Campus = new PushpayCampusDto
                {
                    Key = "test",
                    Name = "test"
                }
            };

        public static PushpayPaymentDto CreateProcessing() =>
            new PushpayPaymentDto
            {
                Status = "Processing",
                PaymentMethodType = "ACH",
                Account = new PushpayAccount {},
                Payer = new PushpayPayer {},
                Campus = new PushpayCampusDto
                {
                    Key = "test",
                    Name = "test"
                }
            };

        public static PushpayPaymentDto CreateFailed() =>
            new PushpayPaymentDto
            {
                Status = "Failed",
                PaymentMethodType = "ACH",
                Account = new PushpayAccount {},
                Payer = new PushpayPayer {},
                Campus = new PushpayCampusDto
                {
                    Key = "test",
                    Name = "test"
                }
            };

        public static List<PushpayPaymentDto> CreateProcessingList() =>
            new List<PushpayPaymentDto>
            {
                new PushpayPaymentDto
                {
                    Status = "Success",
                    PaymentMethodType = "ACH",
                    Account = new PushpayAccount { },
                    Payer = new PushpayPayer { },
                    Campus = new PushpayCampusDto
                    {
                        Key = "test",
                        Name = "test"
                    }
                },
                new PushpayPaymentDto
                {
                    Status = "Processing",
                    PaymentMethodType = "ACH",
                    Account = new PushpayAccount { },
                    Payer = new PushpayPayer { },
                    Campus = new PushpayCampusDto
                    {
                        Key = "test",
                        Name = "test"
                    }
                }
            };
    }
}
