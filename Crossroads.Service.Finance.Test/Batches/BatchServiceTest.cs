using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Services;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Mock;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

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
        public void ShouldBuildDonationBatchObject()
        {
            var timestamp = DateTime.Now;
            var donationsMock = MpDonationsMock.CreateList();
            var chargesMock = PaymentDtoMock.CreateList();
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

            _donationRepository.Setup(r => r.GetDonationByTransactionCode("PP-1a")).Returns(Task.FromResult(donationsMock[0]));
            _donationRepository.Setup(r => r.GetDonationByTransactionCode("PP-2b")).Returns(Task.FromResult(donationsMock[1]));
            _donationRepository.Setup(r => r.GetDonationByTransactionCode("PP-3c")).Returns(Task.FromResult(donationsMock[2]));

            var result = _fixture.BuildDonationBatch(chargesMock, "depositName", timestamp, "transferId").Result;

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

            _batchRepository.Setup(r => r.CreateDonationBatch(It.IsAny<MpDonationBatch>())).Returns(Task.FromResult(mpBatch));
            _mapper.Setup(m => m.Map<MpDonationBatch>(It.IsAny<DonationBatchDto>())).Returns(mpBatch);
            _mapper.Setup(m => m.Map<DonationBatchDto>(It.IsAny<MpDonationBatch>())).Returns(batchnew);

            var result = _fixture.SaveDonationBatch(batch).Result;

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

            _batchRepository.Setup(r => r.CreateDonationBatch(It.IsAny<MpDonationBatch>())).Returns(Task.FromResult(mpBatch));
            _mapper.Setup(m => m.Map<MpDonationBatch>(It.IsAny<DonationBatchDto>())).Returns(mpBatch);
            _mapper.Setup(m => m.Map<DonationBatchDto>(It.IsAny<MpDonationBatch>())).Returns(batchnew);

            _fixture.UpdateDonationBatch(batch);

            _batchRepository.Verify();
        }
    }
}
