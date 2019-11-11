using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services;
using MinistryPlatform.Interfaces;
using Moq;
using Xunit;
using Pushpay.Client;
using Crossroads.Web.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Congregations;
using MinistryPlatform.Donors;
using Pushpay.Models;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;
using Mock;
using Utilities.Logging;

namespace Crossroads.Service.Finance.Test.Pushpay
{
    public class PushpayServiceTest
    {
        private readonly Mock<IPushpayClient> _pushpayClient;
        private readonly Mock<IDonationService> _donationService;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IConfigurationWrapper> _configurationWrapper;
        private readonly Mock<IRecurringGiftRepository> _recurringGiftRepository;
        private readonly Mock<IProgramRepository> _programRepository;
        private readonly Mock<IContactRepository> _contactRepository;
        private readonly Mock<IDonorRepository> _donorRepository;
        private readonly Mock<IWebhooksRepository> _webhooksRespository;
        private readonly Mock<IGatewayService> _gatewayService;
        private readonly Mock<IDataLoggingService> _dataLoggingService;
        private readonly Mock<IDonationDistributionRepository> _donationDistributionRepository;
        private readonly Mock<ICongregationRepository> _congregationRepository;

        private readonly IPushpayService _fixture;

        public PushpayServiceTest()
        {
            _pushpayClient = new Mock<IPushpayClient>();
            _donationService = new Mock<IDonationService>();
            _mapper = new Mock<IMapper>();
            _configurationWrapper = new Mock<IConfigurationWrapper>();
            _recurringGiftRepository = new Mock<IRecurringGiftRepository>();
            _programRepository = new Mock<IProgramRepository>();
            _contactRepository = new Mock<IContactRepository>();
            _donorRepository = new Mock<IDonorRepository>();
            _webhooksRespository = new Mock<IWebhooksRepository>();
            _gatewayService = new Mock<IGatewayService>();
            _dataLoggingService = new Mock<IDataLoggingService>();
            _donationDistributionRepository = new Mock<IDonationDistributionRepository>();
            _congregationRepository = new Mock<ICongregationRepository>();

            _fixture = new PushpayService(_pushpayClient.Object, _donationService.Object, _mapper.Object,
                                          _configurationWrapper.Object, _recurringGiftRepository.Object,
                                          _programRepository.Object, _contactRepository.Object, _donorRepository.Object,
                                          _webhooksRespository.Object, _gatewayService.Object, _dataLoggingService.Object,
                                          _donationDistributionRepository.Object, _congregationRepository.Object);
        }

        [Fact]
        public void ShouldUpdateDonationStatusPendingFromPushpay()
        {
            string transactionCode = "87234354pending";
            var webhookMock = Mock.PushpayStatusChangeRequestMock.Create();
            _pushpayClient.Setup(r => r.GetPayment(webhookMock)).Returns(Task.FromResult(Mock.PushpayPaymentDtoMock.CreateProcessing()));
            _donationService.Setup(r => r.GetDonationByTransactionCode(It.IsAny<string>())).Returns(Task.FromResult(Mock.DonationDtoMock.CreatePending(transactionCode)));
            _donationService.Setup(r => r.CreateDonorAccount(It.IsAny<MpDonorAccount>())).Returns(Task.FromResult(Mock.MpDonorAccountMock.Create()));
            _donationService.Setup(r => r.Update(It.IsAny<DonationDto>())).Returns(Task.FromResult(Mock.DonationDtoMock.CreateSucceeded("a")));

            var result = _fixture.UpdateDonationDetailsFromPushpay(webhookMock).Result;

            // is pending
            Assert.Equal(1, result.DonationStatusId);
        }

        [Fact]
        public void ShouldUpdateDonationStatusSuccessFromPushpay()
        {
            string transactionCode = "87234354v";
            var webhookMock = Mock.PushpayStatusChangeRequestMock.Create();
            _pushpayClient.Setup(r => r.GetPayment(webhookMock)).Returns(Task.FromResult(Mock.PushpayPaymentDtoMock.CreateSuccess()));
            _donationService.Setup(r => r.GetDonationByTransactionCode(It.IsAny<string>())).Returns(Task.FromResult(Mock.DonationDtoMock.CreatePending(transactionCode)));
            _donationService.Setup(r => r.CreateDonorAccount(It.IsAny<MpDonorAccount>())).Returns(Task.FromResult(Mock.MpDonorAccountMock.Create()));
            _donationService.Setup(r => r.Update(It.IsAny<DonationDto>())).Returns(Task.FromResult(Mock.DonationDtoMock.CreateSucceeded("a")));

            var result = _fixture.UpdateDonationDetailsFromPushpay(webhookMock).Result;

            // is success
            Assert.Equal(4, result.DonationStatusId);
        }

        [Fact]
        public void ShouldUpdateDonationStatusDeclinedFromPushpay()
        {
            string transactionCode = "87234354v";
            var webhookMock = Mock.PushpayStatusChangeRequestMock.Create();
            _pushpayClient.Setup(r => r.GetPayment(webhookMock)).Returns(Task.FromResult(Mock.PushpayPaymentDtoMock.CreateFailed()));
            _donationService.Setup(r => r.GetDonationByTransactionCode(It.IsAny<string>())).Returns(Task.FromResult(Mock.DonationDtoMock.CreatePending(transactionCode)));
            _donationService.Setup(r => r.CreateDonorAccount(It.IsAny<MpDonorAccount>())).Returns(Task.FromResult(Mock.MpDonorAccountMock.Create()));
            _donationService.Setup(r => r.Update(It.IsAny<DonationDto>())).Returns(Task.FromResult(Mock.DonationDtoMock.CreateSucceeded("a")));

            var result = _fixture.UpdateDonationDetailsFromPushpay(webhookMock).Result;

            // is failed
            Assert.Equal(3, result.DonationStatusId);
        }

        [Fact]
        public void ShouldGetDepositsByDateRange()
        {
            // Arrange
            var startDate = new DateTime(2017, 12, 12);
            var endDate = new DateTime(2017, 12, 17);

            var pushpayDepositDtos = new List<PushpaySettlementDto>
            {
                new PushpaySettlementDto()
            };

            var depositDtos = new List<SettlementEventDto>
            {
                new SettlementEventDto()
            };

            _mapper.Setup(m => m.Map<List<SettlementEventDto>>(It.IsAny<List<PushpaySettlementDto>>())).Returns(depositDtos);
            _pushpayClient.Setup(m => m.GetDepositsByDateRange(startDate, endDate)).Returns(Task.FromResult(pushpayDepositDtos));

            // Act
            var result = _fixture.GetDepositsByDateRange(startDate, endDate).Result;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        // no match, assign to default contact
        public void ShouldCreateRecurringGiftNoMatch()
        {
            var link = "https://sandbox-api.pushpay.io/v1/merchant/NzkwMjY0NTpuSzZwaUgzakc4WHdZVy1xd0ZVNnlzTlF2aTg/recurringpayment/f6iVOR9VyItfcpuVMnx1gg";
            var webhook = new PushpayWebhook()
            {
                Events = new List<PushpayWebhookEvent>(){
                    new PushpayWebhookEvent()
                    {
                        Links = new PushpayWebhookLinks()
                        {
                            RecurringPayment = link
                        }
                    }
                }
            };
            var pushpayRecurringGift = new PushpayRecurringGiftDto() {
                Payer = new PushpayPayer()
                {
                    Key = "payerkey",
                    Address = new PushpayAddress()
                },
                Account = new PushpayAccount()
                {
                    Reference = "0102010111000"
                },
                Fund = new PushpayFundDto()
                {
                    Code = "I'm In"
                },
                Links = new PushpayLinksDto(),
                PaymentMethodType = "ACH"
            };
            var mpRecurringGift = new MpRecurringGift()
            {
                DonorId = 1
            };
            var mockDonorAccount = MpDonorAccountMock.Create();
            var mockHousehold = new MpHousehold()
            {
                CongregationId = 1
            };
            _pushpayClient.Setup(m => m.GetRecurringGift(link)).Returns(Task.FromResult(pushpayRecurringGift));
            // return null donor
            _donorRepository.Setup(m => m.GetDonorIdByProcessorId(It.IsAny<string>())).Returns(Task.FromResult((int?)null));
            _donorRepository.Setup(m => m.GetDonorByDonorId(It.IsAny<int>()))
                              .Returns(Task.FromResult((MpDonor)null));
            // don't match
            _contactRepository.Setup(m => m.MatchContact(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                              .Returns((MpDonor)null);
            _donationService.Setup(m => m.CreateDonorAccount(It.IsAny<MpDonorAccount>()))
                            .Returns(Task.FromResult(mockDonorAccount));
            _mapper.Setup(m => m.Map<MpRecurringGift>(It.IsAny<PushpayRecurringGiftDto>())).Returns(mpRecurringGift);
            _contactRepository.Setup(m => m.GetHousehold(It.IsAny<int>()))
                              .Returns(mockHousehold);
            _programRepository.Setup(m => m.GetProgramByName(It.IsAny<string>()))
                              .Returns(new MpProgram());
            _recurringGiftRepository.Setup(m => m.CreateRecurringGift(It.IsAny<MpRecurringGift>()));
                               //.Returns(null);
            _mapper.Setup(m => m.Map<RecurringGiftDto>(It.IsAny<MpRecurringGift>()))
                                .Returns(new RecurringGiftDto(){ DonorId = 1 });

            var result = _fixture.CreateRecurringGift(webhook).Result;

            Assert.Equal(1, result.DonorId);
        }

        [Fact]
        // match through processor id (previously manually matched through stored proc)
        public void ShouldCreateRecurringGiftExistingMatch()
        {
            var link = "https://sandbox-api.pushpay.io/v1/merchant/NzkwMjY0NTpuSzZwaUgzakc4WHdZVy1xd0ZVNnlzTlF2aTg/recurringpayment/f6iVOR9VyItfcpuVMnx1gg";
            var viewRecurringPaymentLink =
                "https://sandbox.pushpay.io/pushpay/AzkwMjY1NTpuSzZwaRgzakc5WHdZVy1zd0ZVNnlzTlM2bTg/recurringtransaction/7v0ZiHhZCLm5YH54usQqzA";
            var webhook = new PushpayWebhook()
            {
                Events = new List<PushpayWebhookEvent>(){
                    new PushpayWebhookEvent()
                    {
                        Links = new PushpayWebhookLinks()
                        {
                            RecurringPayment = link,
                            ViewRecurringPayment = viewRecurringPaymentLink
                        }
                    }
                }
            };
            var pushpayRecurringGift = new PushpayRecurringGiftDto()
            {
                Payer = new PushpayPayer()
                {
                    Key = "payerkey",
                    Address = new PushpayAddress()
                },
                Account = new PushpayAccount()
                {
                    Reference = "0102010111000"
                },
                Fund = new PushpayFundDto()
                {
                    Code = "I'm In"
                },
                Links = new PushpayLinksDto(),
                PaymentMethodType = "ACH"
            };
            var mpRecurringGift = new MpRecurringGift()
            {
                DonorId = 234
            };
            var mpDonor = new MpDonor()
            {
                DonorId = 234
            };
            var mockDonorAccount = MpDonorAccountMock.Create();
            var mockHousehold = new MpHousehold()
            {
                CongregationId = 1
            };
            _pushpayClient.Setup(m => m.GetRecurringGift(link)).Returns(Task.FromResult(pushpayRecurringGift));
            // return null donor
            int? donorId = 1234567;
            _donorRepository.Setup(m => m.GetDonorIdByProcessorId(It.IsAny<string>())).Returns(Task.FromResult(donorId));
            _donorRepository.Setup(m => m.GetDonorByDonorId(It.IsAny<int>()))
                              .Returns(Task.FromResult(mpDonor));
            _contactRepository.Setup(m => m.GetHousehold(It.IsAny<int>()))
                              .Returns(mockHousehold);
            _programRepository.Setup(m => m.GetProgramByName(It.IsAny<string>()))
                              .Returns(new MpProgram());
            _recurringGiftRepository.Setup(m => m.CreateRecurringGift(It.IsAny<MpRecurringGift>()));
            _mapper.Setup(m => m.Map<RecurringGiftDto>(It.IsAny<MpRecurringGift>()))
                                .Returns(new RecurringGiftDto() { DonorId = 1 });
            _donationService.Setup(m => m.CreateDonorAccount(It.IsAny<MpDonorAccount>()))
                            .Returns(Task.FromResult(mockDonorAccount));
            _mapper.Setup(m => m.Map<MpRecurringGift>(It.IsAny<PushpayRecurringGiftDto>())).Returns(mpRecurringGift);

            var result = _fixture.CreateRecurringGift(webhook).Result;
            Assert.Equal(1, result.DonorId);
        }

        [Fact]
        // match through stored proc
        public void ShouldCreateRecurringGiftManuallyMatch()
        {
            var link = "https://sandbox-api.pushpay.io/v1/merchant/NzkwMjY0NTpuSzZwaUgzakc4WHdZVy1xd0ZVNnlzTlF2aTg/recurringpayment/f6iVOR9VyItfcpuVMnx1gg";
            var webhook = new PushpayWebhook()
            {
                Events = new List<PushpayWebhookEvent>(){
                        new PushpayWebhookEvent()
                        {
                            Links = new PushpayWebhookLinks()
                            {
                                RecurringPayment = link
                            }
                        }
                    }
            };
            var pushpayRecurringGift = new PushpayRecurringGiftDto()
            {
                Payer = new PushpayPayer()
                {
                    Key = "payerkey",
                    Address = new PushpayAddress()
                },
                Account = new PushpayAccount()
                {
                    Reference = "0102010111000"
                },
                Fund = new PushpayFundDto()
                {
                    Code = "I'm In"
                },
                Links = new PushpayLinksDto(),
                PaymentMethodType = "ACH"
            };
            var mpRecurringGift = new MpRecurringGift()
            {
                DonorId = 1
            };
            var mockDonorAccount = MpDonorAccountMock.Create();
            var mockDonor = new MpDonor()
            {
                DonorId = 789
            };
            var mockHousehold = new MpHousehold()
            {
                CongregationId = 1
            };
            _pushpayClient.Setup(m => m.GetRecurringGift(link)).Returns(Task.FromResult(pushpayRecurringGift));
            // return null donor
            _donorRepository.Setup(m => m.GetDonorIdByProcessorId(It.IsAny<string>())).Returns(Task.FromResult((int?)null));
            _donorRepository.Setup(m => m.GetDonorByDonorId(It.IsAny<int>()))
                .Returns(Task.FromResult((MpDonor)null));
            // don't match
            _contactRepository.Setup(m => m.MatchContact(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                  .Returns((MpDonor)null);
            _donationService.Setup(m => m.CreateDonorAccount(It.IsAny<MpDonorAccount>()))
                                .Returns(Task.FromResult(mockDonorAccount));
            _mapper.Setup(m => m.Map<MpRecurringGift>(It.IsAny<PushpayRecurringGiftDto>())).Returns(mpRecurringGift);
            _contactRepository.Setup(m => m.GetHousehold(It.IsAny<int>()))
                                  .Returns(mockHousehold);
            _programRepository.Setup(m => m.GetProgramByName(It.IsAny<string>()))
                                  .Returns(new MpProgram());
            _mapper.Setup(m => m.Map<RecurringGiftDto>(It.IsAny<MpRecurringGift>()))
                                .Returns(new RecurringGiftDto() { DonorId = 1 });

            var result = _fixture.CreateRecurringGift(webhook).Result;

            Assert.Equal(1, result.DonorId);
        }

        [Fact]
        public void ShouldUpdateRecurringGiftToActive()
        {
            var link = "https://sandbox-api.pushpay.io/v1/merchant/NzkwMjY0NTpuSzZwaUgzakc4WHdZVy1xd0ZVNnlzTlF2aTg/recurringpayment/f6iVOR9VyItfcpuVMnx1gg";
            var webhook = new PushpayWebhook()
            {
                Events = new List<PushpayWebhookEvent>(){
                    new PushpayWebhookEvent()
                    {
                        Links = new PushpayWebhookLinks()
                        {
                            RecurringPayment = link
                        }
                    }
                }
            };
            var pushpayRecurringGift = new PushpayRecurringGiftDto()
            {
                Payer = new PushpayPayer()
                {
                    Key = "payerkey",
                    Address = new PushpayAddress()
                },
                Account = new PushpayAccount()
                {
                    Reference = "0102010111000"
                },
                Fund = new PushpayFundDto()
                {
                    Code = "I'm In"
                },
                Links = new PushpayLinksDto(),
                Status = "Active",
                PaymentMethodType = "ACH"
            };
            var mpRecurringGift = new MpRecurringGift()
            {
                DonorId = 1,
                RecurringGiftStatusId = 1
            };
            var mockDonorAccount = MpDonorAccountMock.Create();
            var mockHousehold = new MpHousehold()
            {
                CongregationId = 1
            };
            var mpDonor = new MpDonor()
            {
                DonorId = 1
            };
            _pushpayClient.Setup(m => m.GetRecurringGift(link)).Returns(Task.FromResult(pushpayRecurringGift));
            _recurringGiftRepository.Setup(m => m.FindRecurringGiftBySubscriptionId(pushpayRecurringGift.PaymentToken)).Returns(Task.FromResult((MpRecurringGift)mpRecurringGift));
            // return null donor
            int? donorId = 1234567;
            _donorRepository.Setup(m => m.GetDonorIdByProcessorId(It.IsAny<string>())).Returns(Task.FromResult(donorId));
            _donorRepository.Setup(m => m.GetDonorByDonorId(It.IsAny<int>()))
                .Returns(Task.FromResult(mpDonor));
            // don't match
            _contactRepository.Setup(m => m.MatchContact(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                              .Returns((MpDonor)null);
            _donationService.Setup(m => m.UpdateDonorAccount(It.IsAny<JObject>()));
            _mapper.Setup(m => m.Map<MpRecurringGift>(It.IsAny<PushpayRecurringGiftDto>())).Returns(mpRecurringGift);
            _contactRepository.Setup(m => m.GetHousehold(It.IsAny<int>()))
                              .Returns(mockHousehold);
            _programRepository.Setup(m => m.GetProgramByName(It.IsAny<string>()))
                              .Returns(new MpProgram());
            _recurringGiftRepository.Setup(m => m.UpdateRecurringGift(It.IsAny<JObject>()));
            _mapper.Setup(m => m.Map<RecurringGiftDto>(It.IsAny<MpRecurringGift>()))
                                .Returns(new RecurringGiftDto() { DonorId = 1 });

            var result = _fixture.UpdateRecurringGift(webhook);
        }


        [Fact]
        public void ShouldGetRecurringGiftNotes()
        {
            var pushpayRecurringGift = new PushpayRecurringGiftDto()
            {
                Payer = new PushpayPayer()
                {
                    FirstName = "Dez",
                    LastName = "Bryant",
                    MobileNumber = "+16536659090",
                    EmailAddress = "dez@cowboys.com",
                    Address = new PushpayAddress() {
                        AddressLine1 = "342 Main Street",
                        AddressLine2 = "Apt. 6",
                        City = "Dallas",
                        State = "TX",
                        Zip = "83566",
                        Country = "US"
                   }
                }
            };
            var result = _fixture.GetRecurringGiftNotes(pushpayRecurringGift);
            var expected = "First Name: Dez Last Name: Bryant Phone: (653) 665-9090 Email: dez@cowboys.com ";
            expected += "Address1: 342 Main Street Address2: Apt. 6 City, State Zip: Dallas, TX 83566 Country: USA";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ShouldGetRecurringGiftNotesNoAddress()
        {
            var pushpayRecurringGift = new PushpayRecurringGiftDto()
            {
                Payer = new PushpayPayer()
                {
                    FirstName = "Dez",
                    LastName = "Bryant",
                    MobileNumber = "+16536659090",
                    EmailAddress = "dez@cowboys.com",
                    Address = new PushpayAddress()
                    {
                        Zip = "83566",
                        Country = "US"
                    }
                }
            };
            var result = _fixture.GetRecurringGiftNotes(pushpayRecurringGift);
            var expected = "First Name: Dez Last Name: Bryant Phone: (653) 665-9090 Email: dez@cowboys.com ";
            expected += "Address1: Street Address Not Provided Address2:  City, State Zip: ,  83566 Country: USA";
            Assert.Equal(expected, result);
        }
    }
}
