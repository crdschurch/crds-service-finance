using System;
using System.Collections.Generic;
using MinistryPlatform.Models;

namespace Mock
{
    public static class MockPushPayRecurringSchedules
    {
      public static List<MpRawPushPayRecurringSchedules> GetListOfOne()
        {
            return new List<MpRawPushPayRecurringSchedules>
            {
                new MpRawPushPayRecurringSchedules
                {
                    RecurringGiftScheduleId = 1,
                    IsProcessed = false,
                    RawJson = @"{
                                  ""schedule"": {
                                    ""frequency"": ""Monthly"",
                                    ""startDate"": ""2018-07-13T00:00:00-04:00"",
                                    ""endType"": ""Never"",
                                    ""nextPaymentDate"": ""2018-08-13T19:00:00Z""
                                  },
                                  ""status"": ""Active"",
                                  ""paymentToken"": ""ashfuiasdhfiu"",
                                  ""amount"": {
                                    ""amount"": ""12.47"",
                                    ""currency"": ""USD"",
                                    ""details"": {
                                      ""base"": ""12.47""
                                    }
                                  },
                                  ""payer"": {
                                    ""key"": ""asdfauwfguyewagfyui"",
                                    ""emailAddress"": ""test@test.com"",
                                    ""emailAddressVerified"": true,
                                    ""mobileNumberVerified"": false,
                                    ""fullName"": ""Test McPherson"",
                                    ""firstName"": ""Test"",
                                    ""lastName"": ""McPherson"",
                                    ""address"": {
                                      ""country"": ""US""
                                    },
                                    ""updatedOn"": ""2017-11-28T18:01:03Z"",
                                    ""role"": ""individual"",
                                    ""payerType"": ""Registered"",
                                    ""_links"": {
                                      ""self"": {
                                        ""href"": ""http://test/test""
                                      }
                                    }
                                  },
                                  ""recipient"": {
                                    ""key"": ""3204983209483029adsaafsd"",
                                    ""handle"": ""someChurch"",
                                    ""name"": ""church"",
                                    ""role"": ""merchant""
                                  },
                                  ""fields"": [
                                    {
                                      ""key"": ""24571945"",
                                      ""value"": ""Test McPherson"",
                                      ""label"": ""Full name""
                                    },
                                    {
                                      ""key"": ""37249873298"",
                                      ""value"": ""test@test.com"",
                                      ""label"": ""Email""
                                    },
                                    {
                                      ""key"": ""3240983209"",
                                      ""value"": ""Tithes"",
                                      ""label"": ""Fund""
                                    }
                                  ],
                                  ""createdOn"": ""2018-07-13T15:42:51Z"",
                                  ""updatedOn"": ""2018-07-16T18:44:54Z"",
                                  ""paymentMethodType"": ""CreditCard"",
                                  ""source"": ""LoggedInWeb"",
                                  ""card"": {
                                    ""reference"": ""4111-11....1111"",
                                    ""brand"": ""VISA"",
                                    ""logo"": """"
                                  },
                                  ""ipAddress"": """",
                                  ""externalLinks"": [
                                    {
                                      ""application"": {
                                        ""name"": ""Ok"",
                                        ""type"": ""CMS""
                                      },
                                      ""relationship"": ""Nope"",
                                      ""value"": ""90""
                                    }
                                  ],
                                  ""fund"": {
                                    ""key"": ""AYUSDGASUYIGD"",
                                    ""name"": ""Tithes"",
                                    ""code"": ""Tithes"",
                                    ""taxDeductible"": true
                                  },
                                  ""campus"": {
                                    ""name"": ""JustSomeChurch"",
                                    ""key"": ""dsfhuiwaehfuiohafuio""
                                  },
                                  ""_links"": {
                                    ""self"": {
                                      ""href"": """"
                                    },
                                    ""merchant"": {
                                      ""href"": """"
                                    },
                                    ""payments"": {
                                      ""href"": """"
                                    },
                                    ""merchantviewrecurringpayment"": {
                                      ""href"": """"
                                    },
                                    ""donorviewrecurringpayment"": {
                                      ""href"": """"
                                    }
                                  }
                                }",
                    TimeCreated = DateTime.Now
                }
            };
        }
    }
}