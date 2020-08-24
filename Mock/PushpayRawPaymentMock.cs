using System;
using System.Collections.Generic;
using System.Text;
using MinistryPlatform.Models;

namespace Mock
{
	public static class PushpayRawPaymentMock
	{
		public static string GetPayment()
		{
			return "{\r\n  \"status\": \"Success\",\r\n  \"recurringPaymentToken\": \"111111blahblahAAAAAA\",\r\n  \"transactionId\": \"1111122222\",\r\n  \"paymentToken\": \"222222222BBBBBBBBBtest\",\r\n  \"amount\": {\r\n    \"amount\": \"4.91\",\r\n    \"currency\": \"USD\",\r\n    \"details\": {\r\n      \"base\": \"4.91\"\r\n    }\r\n  },\r\n  \"payer\": {\r\n    \"key\": \"CCCCCCCCCC3333333333TEST\",\r\n    \"emailAddress\": \"frodo.baggins@shirenet.com\",\r\n    \"emailAddressVerified\": false,\r\n    \"mobileNumber\": \"+15555551234\",\r\n    \"mobileNumberVerified\": true,\r\n    \"fullName\": \"Frodo Baggins\",\r\n    \"firstName\": \"Frodo\",\r\n    \"lastName\": \"Baggins\",\r\n    \"address\": {\r\n      \"country\": \"US\",\r\n      \"zipOrPostCode\": \"55555\"\r\n    },\r\n    \"updatedOn\": \"2017-12-20T15:41:14Z\",\r\n    \"role\": \"individual\",\r\n    \"payerType\": \"Registered\",\r\n    \"exportKey\": \"aaaaaaaa-2222-3333-bbbb-dddddddddd\",\r\n    \"_links\": {\r\n      \"self\": {\r\n        \"href\": \"https://sandbox-api.pushpay.io/v1/merchant/randommerchantkey1/community/randomcommunitykey2\"\r\n      }\r\n    }\r\n  },\r\n  \"recipient\": {\r\n    \"key\": \"aaaaaaafrodolivescccccccc\",\r\n    \"handle\": \"bucklandtrading\",\r\n    \"name\": \"Buckland Trading\",\r\n    \"role\": \"merchant\"\r\n  },\r\n  \"fields\": [\r\n    {\r\n      \"key\": \"11112222\",\r\n      \"value\": \"Frodo Baggins\",\r\n      \"label\": \"Full name\"\r\n    },\r\n    {\r\n      \"key\": \"33334444\",\r\n      \"value\": \"frodo.baggins@shirenet.com\",\r\n      \"label\": \"Email\"\r\n    },\r\n    {\r\n      \"key\": \"55556666\",\r\n      \"value\": \"Tithes & Contributions\",\r\n      \"label\": \"Fund\"\r\n    },\r\n    {\r\n      \"key\": \"1234567654321\",\r\n      \"value\": \"Breeland\",\r\n      \"label\": \"Breeland\"\r\n    }\r\n  ],\r\n  \"createdOn\": \"2020-08-20T17:52:04Z\",\r\n  \"updatedOn\": \"2020-08-20T17:52:54Z\",\r\n  \"paymentMethodType\": \"CreditCard\",\r\n  \"source\": \"ScheduledPayment\",\r\n  \"card\": {\r\n    \"reference\": \"5555-55....55555\",\r\n    \"brand\": \"VISA\",\r\n    \"logo\": \"https://sandbox.pushpay.io/Content/PushpayWeb/Images/Interface/@2x/PaymentMethods/visa.png\"\r\n  },\r\n  \"ipAddress\": \"111.11.111.111\",\r\n  \"externalLinks\": [\r\n    {\r\n      \"application\": {\r\n        \"name\": \"ThinkMinistry\",\r\n        \"type\": \"ChurchManagementSystem\"\r\n      },\r\n      \"relationship\": \"fund_id\",\r\n      \"value\": \"0\"\r\n    },\r\n    {\r\n      \"application\": {\r\n        \"name\": \"Palantar\",\r\n        \"type\": \"ChurchManagementSystem\"\r\n      },\r\n      \"relationship\": \"person_id\",\r\n      \"value\": \"4455566\"\r\n    },\r\n    {\r\n      \"application\": {\r\n        \"name\": \"Palantar\",\r\n        \"type\": \"ChurchManagementSystem\"\r\n      },\r\n      \"relationship\": \"transaction_id\",\r\n      \"value\": \"6666667777777\"\r\n    }\r\n  ],\r\n  \"fund\": {\r\n    \"key\": \"111gollum222\",\r\n    \"name\": \"ShireDefense\",\r\n    \"code\": \"ShireDefense\",\r\n    \"taxDeductible\": true\r\n  },\r\n  \"campus\": {\r\n    \"name\": \"TheShire\",\r\n    \"key\": \"aaaaaabbbbbb11111112222222\"\r\n  },\r\n  \"_links\": {\r\n    \"self\": {\r\n      \"href\": \"https://sandbox-api.pushpay.io/v1/merchant/randommerchantkey1/payment/randomtransactionkey123\"\r\n    },\r\n    \"merchant\": {\r\n      \"href\": \"https://sandbox-api.pushpay.io/v1/merchant/frandommerchantkey1\"\r\n    },\r\n    \"recurringpayment\": {\r\n      \"href\": \"https://sandbox-api.pushpay.io/v1/merchant/randommerchantkey1/recurringpayment/recurringpaymentkey333\"\r\n    },\r\n    \"merchantviewrecurringpayment\": {\r\n      \"href\": \"https://sandbox.pushpay.io/pushpay/randommerchantkey1/recurringtransaction/recurringpaymentkey333\"\r\n    },\r\n    \"donorviewrecurringpayment\": {\r\n      \"href\": \"https://sandbox.pushpay.io/pushpay/pay/randommerchantkey1/recurring/view/recurringpaymentkey333\"\r\n    },\r\n    \"payments\": {\r\n      \"href\": \"https://sandbox-api.pushpay.io/v1/merchant/randommerchantkey1/payments\"\r\n    },\r\n    \"merchantviewpayment\": {\r\n      \"href\": \"https://sandbox.pushpay.io/pushpay/randommerchantkey1/transaction/randomtransactionkey123\"\r\n    },\r\n    \"donorviewpayment\": {\r\n      \"href\": \"https://sandbox.pushpay.io/pushpay/transaction/randomtransactionkey123\"\r\n    }\r\n  }\r\n}";
		}

		public static MpRawDonation GetRawDonation()
		{
			var mpRawDonation = new MpRawDonation
			{
				DonationId = 1,
				IsProcessed = false,
				RawJson = GetPayment(),
				TimeCreated = DateTime.Now
			};

			return mpRawDonation;
		}
	}
}
