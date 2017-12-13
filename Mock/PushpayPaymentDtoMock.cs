using Pushpay.Models;

namespace Mock
{
    public class PushpayPaymentDtoMock
    {
        public static PushpayPaymentDto CreateSuccess() =>
            new PushpayPaymentDto
            {
                Status = "Success"
            };

        public static PushpayPaymentDto CreateProcessing() =>
            new PushpayPaymentDto
            {
                Status = "Processing"
            };

        public static PushpayPaymentDto CreateFailed() =>
            new PushpayPaymentDto
            {
                Status = "Failed"
            };
    }
}
