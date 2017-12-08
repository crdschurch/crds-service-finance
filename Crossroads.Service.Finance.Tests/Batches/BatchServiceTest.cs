using System;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Mock;
using Xunit;
using Moq;

namespace Crossroads.Service.Finance.Test.Batches
{
    public class BatchServiceTest
    {
        private readonly Mock<IDonationRepository> _donationRepository;
        private readonly Mock<IBatchRepository> _batchRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IConfigurationWrapper> _configurationWrapper;

        private readonly IBatchService _fixture;

        public BatchServiceTest()
        {
            _configurationWrapper = new Mock<IConfigurationWrapper>();
            _donationRepository = new Mock<IDonationRepository>();
            _batchRepository = new Mock<IBatchRepository>();
            _mapper = new Mock<IMapper>();

            _fixture = new BatchService(_donationRepository.Object, _batchRepository.Object, _mapper.Object, _configurationWrapper.Object);
        }

        [Fact]
        public void ShouldCreateDonationBatchObject()
        {
            var timestamp = DateTime.Now;
            var donationsMock = MpDonationsMock.CreateList();
            var chargesMock = PaymentProcessorChargeDtoMock.CreateList();
            var expectedBatch = new DonationBatchDto
            {
                BatchName = "depositName",
                SetupDateTime = timestamp,
                BatchTotalAmount = 90,
                ItemCount = 3,
                BatchEntryType = 10,
                FinalizedDateTime = timestamp,
                DepositId = null,
                ProcessorTransferId = "transferId"
            };

            _donationRepository.Setup(r => r.GetDonationByTransactionCode("1a")).Returns(donationsMock[0]);
            _donationRepository.Setup(r => r.GetDonationByTransactionCode("2b")).Returns(donationsMock[0]);
            _donationRepository.Setup(r => r.GetDonationByTransactionCode("3c")).Returns(donationsMock[0]);

            var result = _fixture.CreateDonationBatch(chargesMock, "depositName", timestamp, "transferId");

            Assert.Equal(expectedBatch.SetupDateTime, timestamp);
            Assert.Equal(expectedBatch.FinalizedDateTime, timestamp);
            Assert.Equal(expectedBatch.ItemCount, result.ItemCount);
            Assert.Equal(expectedBatch.BatchTotalAmount, result.BatchTotalAmount);
        }

        [Fact]
        public void ShouldSaveDonationBatchObject()
        {
            var batch = new DonationBatchDto
            {
                BatchTotalAmount = 20
            };

            var mpBatch = new MpDonationBatch
            {
                Id = 123,
                BatchTotalAmount = 20
            };

            var batchnew = new DonationBatchDto
            {
                Id = 123,
                BatchTotalAmount = 20
            };

            _batchRepository.Setup(r => r.CreateDonationBatch(It.IsAny<MpDonationBatch>())).Returns(mpBatch);
            _mapper.Setup(m => m.Map<MpDonationBatch>(It.IsAny<DonationBatchDto>())).Returns(mpBatch);
            _mapper.Setup(m => m.Map<DonationBatchDto>(It.IsAny<MpDonationBatch>())).Returns(batchnew);

            var result = _fixture.SaveDonationBatch(batch);

            Assert.Equal(result.Id, mpBatch.Id);
            Assert.Equal(result.BatchTotalAmount, mpBatch.BatchTotalAmount);
        }

        [Fact]
        public void ShouldUpdateDonationBatchObject()
        {
            var batch = new DonationBatchDto
            {
                BatchTotalAmount = 20
            };

            var mpBatch = new MpDonationBatch
            {
                Id = 123,
                BatchTotalAmount = 30
            };

            var batchnew = new DonationBatchDto
            {
                Id = 123,
                BatchTotalAmount = 30,
                DepositId = 12
            };

            _batchRepository.Setup(r => r.CreateDonationBatch(It.IsAny<MpDonationBatch>())).Returns(mpBatch);
            _mapper.Setup(m => m.Map<MpDonationBatch>(It.IsAny<DonationBatchDto>())).Returns(mpBatch);
            _mapper.Setup(m => m.Map<DonationBatchDto>(It.IsAny<MpDonationBatch>())).Returns(batchnew);

            _fixture.UpdateDonationBatch(batch);

            _batchRepository.Verify();
        }
    }
}
