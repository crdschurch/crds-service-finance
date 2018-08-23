using System;
using System.Collections.Generic;
using Crossroads.Service.Finance.Models;

namespace Mock
{
    public class PushpayStatusChangeRequestMock
    {
        public static PushpayWebhook Create() =>
            new PushpayWebhook()
            {
                Subscription = "https://sandbox-api.pushpay.io/v1/merchant/NzkwMjY0NTpuSzZwaUgzakc4WHdZVy1xd0ZVNnlzTlF2aTg/webhook/8DWULX3yULJimB3oqaYj8w",
                Events = new List<PushpayWebhookEvent>
                {
                new PushpayWebhookEvent
                    {
                        Date = DateTime.Parse("2017-12-01T15:55:51Z"),
                        EventType = "payment_status_changed",
                        EntityType = "Payment",
                        From = "processing",
                        To = "success",
                        Links = new PushpayWebhookLinks() {
                            Payment = "https://sandbox-api.pushpay.io/v1/merchant/NzkwMjY0NTpuSzZwaUgzakc4WHdZVy1xd0ZVNnlzTlF2aTg/payment/5dcdaPH0wkqvnd4LmB7dEg",
                            Merchant = "https://sandbox-api.pushpay.io/v1/merchant/NzkwMjY0NTpuSzZwaUgzakc4WHdZVy1xd0ZVNnlzTlF2aTg"
                        }
                    }
                },
                IncomingTimeUtc = DateTime.Now
            };
    }
}
