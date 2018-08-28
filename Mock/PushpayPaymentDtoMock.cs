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
                Payer = new PushpayPayer {}
            };

        public static PushpayPaymentDto CreateProcessing() =>
            new PushpayPaymentDto
            {
                Status = "Processing",
                PaymentMethodType = "ACH",
                Account = new PushpayAccount {},
                Payer = new PushpayPayer {}
            };

        public static PushpayPaymentDto CreateFailed() =>
            new PushpayPaymentDto
            {
                Status = "Failed",
                PaymentMethodType = "ACH",
                Account = new PushpayAccount {},
                Payer = new PushpayPayer {}
            };
    }
}
