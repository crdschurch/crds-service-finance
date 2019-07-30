using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Service.Finance.Services.Exports;
using MinistryPlatform.Adjustments;
using Xunit;

namespace Crossroads.Service.Finance.Test.Exports
{
    public class ExportServiceTest
    {
        private readonly Mock<IAdjustmentRepository> _adjustmentRepository;
        private readonly Mock<IMapper> _mapper;

        private readonly IExportService _fixture;

        public ExportServiceTest()
        {
            _adjustmentRepository = new Mock<IAdjustmentRepository>();
            _mapper = new Mock<IMapper>();

            _fixture = new ExportService(_adjustmentRepository.Object, _mapper.Object);
        }

        [Fact]
        public void ShouldCreateJournalEntries()
        {
            // Arrange


            // Act
            _fixture.CreateJournalEntries();

            // Assert
            
        }
    }
}
