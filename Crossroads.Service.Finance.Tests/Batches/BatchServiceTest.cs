using AutoMapper;
using Crossroads.Service.Finance.Services.Batches;
using MinistryPlatform.Batches;
using MinistryPlatform.Donations;
using Xunit;
using Moq;

namespace Crossroads.Service.Finance.Test.Batches
{
    public class BatchServiceTest
    {
        private readonly Mock<IDonationRepository> _donationRepository;
        private readonly Mock<IBatchRepository> _batchRepository;
        private readonly Mock<IMapper> _mapper;

        private readonly IBatchService _fixture;

        public BatchServiceTest()
        {
            _donationRepository = new Mock<IDonationRepository>();
            _batchRepository = new Mock<IBatchRepository>();
            _mapper = new Mock<IMapper>();

            _fixture = new BatchService(_donationRepository.Object, _batchRepository.Object, _mapper.Object);
        }

        [Fact]
        public void ShouldCreateDonationBatchObject()
        {
        }
    }
}
