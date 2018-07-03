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
using Crossroads.Service.Finance.Models;
using Pushpay.Models;
using MinistryPlatform.Models;

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

            _fixture = new PushpayService(_pushpayClient.Object, _donationService.Object, _mapper.Object,
                                          _configurationWrapper.Object, _recurringGiftRepository.Object,
                                          _programRepository.Object, _contactRepository.Object);
        }

        [Fact]
        public void ShouldUpdateDonationStatusPendingFromPushpay()
        {
            string transactionCode = "87234354pending";
            var webhookMock = Mock.PushpayStatusChangeRequestMock.Create();
            _pushpayClient.Setup(r => r.GetPayment(webhookMock)).Returns(Mock.PushpayPaymentDtoMock.CreateProcessing());
            _donationService.Setup(r => r.GetDonationByTransactionCode(It.IsAny<string>())).Returns(Mock.DonationDtoMock.CreatePending(transactionCode));

            var result = _fixture.UpdateDonationStatusFromPushpay(webhookMock);

            // is pending
            Assert.Equal(1, result.DonationStatusId);
        }

        [Fact]
        public void ShouldUpdateDonationStatusSuccessFromPushpay()
        {
            string transactionCode = "87234354v";
            var webhookMock = Mock.PushpayStatusChangeRequestMock.Create();
            _pushpayClient.Setup(r => r.GetPayment(webhookMock)).Returns(Mock.PushpayPaymentDtoMock.CreateSuccess());
            _donationService.Setup(r => r.GetDonationByTransactionCode(It.IsAny<string>())).Returns(Mock.DonationDtoMock.CreatePending(transactionCode));

            var result = _fixture.UpdateDonationStatusFromPushpay(webhookMock);

            // is success
            Assert.Equal(4, result.DonationStatusId);
        }

        [Fact]
        public void ShouldUpdateDonationStatusDeclinedFromPushpay()
        {
            string transactionCode = "87234354v";
            var webhookMock = Mock.PushpayStatusChangeRequestMock.Create();
            _pushpayClient.Setup(r => r.GetPayment(webhookMock)).Returns(Mock.PushpayPaymentDtoMock.CreateFailed());
            _donationService.Setup(r => r.GetDonationByTransactionCode(It.IsAny<string>())).Returns(Mock.DonationDtoMock.CreatePending(transactionCode));

            var result = _fixture.UpdateDonationStatusFromPushpay(webhookMock);

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
            _pushpayClient.Setup(m => m.GetDepositsByDateRange(startDate, endDate)).Returns(pushpayDepositDtos);

            // Act
            var result = _fixture.GetDepositsByDateRange(startDate, endDate);

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
                    Key = "payerkey"
                },
                Account = new PushpayAccount()
                {
                    Reference = "0102010111000"
                },
                Fund = new PushpayFund()
                {
                    Code = "I'm In"
                },
                Links = new PushpayLinksDto()
            };
            var mpRecurringGift = new MpRecurringGift()
            {
                DonorId = 1
            };
            var mockDonorAccount = new MpDonorAccount();
            var mockHousehold = new MpHousehold()
            {
                CongregationId = 1
            };
            _pushpayClient.Setup(m => m.GetRecurringGift(link)).Returns(pushpayRecurringGift);
            // return null donor
            _contactRepository.Setup(m => m.FindDonorByProcessorId(It.IsAny<string>()))
                              .Returns((MpDonor)null);
            // don't match
            _contactRepository.Setup(m => m.MatchContact(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                              .Returns((MpDonor)null);
            _donationService.Setup(m => m.CreateDonorAccount(It.IsAny<MpDonorAccount>()))
                            .Returns(mockDonorAccount);
            _mapper.Setup(m => m.Map<MpRecurringGift>(It.IsAny<PushpayRecurringGiftDto>())).Returns(mpRecurringGift);
            _contactRepository.Setup(m => m.GetHousehold(It.IsAny<int>()))
                              .Returns(mockHousehold);
            _programRepository.Setup(m => m.GetProgramByName(It.IsAny<string>()))
                              .Returns(new MpProgram());
            _recurringGiftRepository.Setup(m => m.CreateRecurringGift(It.IsAny<MpRecurringGift>()));
                               //.Returns(null);
            _mapper.Setup(m => m.Map<RecurringGiftDto>(It.IsAny<MpRecurringGift>()))
                                .Returns(new RecurringGiftDto(){ DonorId = 1 });

            var result = _fixture.CreateRecurringGift(webhook);

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
                    Key = "payerkey"
                },
                Account = new PushpayAccount()
                {
                    Reference = "0102010111000"
                },
                Fund = new PushpayFund()
                {
                    Code = "I'm In"
                },
                Links = new PushpayLinksDto()
            };
            var mpRecurringGift = new MpRecurringGift()
            {
                DonorId = 234
            };
            var mpDonor = new MpDonor()
            {
                DonorId = 234
            };
            var mockDonorAccount = new MpDonorAccount()
            {
                DonorAccountId = 777
            };
            var mockHousehold = new MpHousehold()
            {
                CongregationId = 1
            };
            _pushpayClient.Setup(m => m.GetRecurringGift(link)).Returns(pushpayRecurringGift);
            // return null donor
            _contactRepository.Setup(m => m.FindDonorByProcessorId(It.IsAny<string>()))
                              .Returns(mpDonor);
            _contactRepository.Setup(m => m.GetHousehold(It.IsAny<int>()))
                              .Returns(mockHousehold);
            _programRepository.Setup(m => m.GetProgramByName(It.IsAny<string>()))
                              .Returns(new MpProgram());
            _recurringGiftRepository.Setup(m => m.CreateRecurringGift(It.IsAny<MpRecurringGift>()));
            _mapper.Setup(m => m.Map<RecurringGiftDto>(It.IsAny<MpRecurringGift>()))
                                .Returns(new RecurringGiftDto() { DonorId = 1 });
            _donationService.Setup(m => m.CreateDonorAccount(It.IsAny<MpDonorAccount>()))
                            .Returns(mockDonorAccount);
            _mapper.Setup(m => m.Map<MpRecurringGift>(It.IsAny<PushpayRecurringGiftDto>())).Returns(mpRecurringGift);

            var result = _fixture.CreateRecurringGift(webhook);
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
                    Key = "payerkey"
                },
                Account = new PushpayAccount()
                {
                    Reference = "0102010111000"
                },
                Fund = new PushpayFund()
                {
                    Code = "I'm In"
                },
                Links = new PushpayLinksDto()
            };
            var mpRecurringGift = new MpRecurringGift()
            {
                DonorId = 1
            };
            var mockDonorAccount = new MpDonorAccount();
            var mockDonor = new MpDonor()
            {
                DonorId = 789
            };
            var mockHousehold = new MpHousehold()
            {
                CongregationId = 1
            };
            _pushpayClient.Setup(m => m.GetRecurringGift(link)).Returns(pushpayRecurringGift);
            // return null donor
            _contactRepository.Setup(m => m.FindDonorByProcessorId(It.IsAny<string>()))
                                  .Returns((MpDonor)null);
            // don't match
            _contactRepository.Setup(m => m.MatchContact(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                  .Returns((MpDonor)null);
            _donationService.Setup(m => m.CreateDonorAccount(It.IsAny<MpDonorAccount>()))
                                .Returns(mockDonorAccount);
            _mapper.Setup(m => m.Map<MpRecurringGift>(It.IsAny<PushpayRecurringGiftDto>())).Returns(mpRecurringGift);
            _contactRepository.Setup(m => m.GetHousehold(It.IsAny<int>()))
                                  .Returns(mockHousehold);
            _programRepository.Setup(m => m.GetProgramByName(It.IsAny<string>()))
                                  .Returns(new MpProgram());
            _contactRepository.Setup(m => m.UpdateProcessor(It.IsAny<int>(), It.IsAny<string>()));
            _mapper.Setup(m => m.Map<RecurringGiftDto>(It.IsAny<MpRecurringGift>()))
                                .Returns(new RecurringGiftDto() { DonorId = 1 });

            var result = _fixture.CreateRecurringGift(webhook);

            Assert.Equal(1, result.DonorId);
        }
    }
}
