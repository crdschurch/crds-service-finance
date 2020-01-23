using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Repositories;
using MinistryPlatform.Models;
using Moq;
using Xunit;

namespace MinistryPlatform.Test.Donations
{
    public class ProgramRepositoryTest
    {
        readonly Mock<IApiUserRepository> _apiUserRepository;
        readonly Mock<IConfigurationWrapper> _configurationWrapper;
        readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        readonly Mock<IMinistryPlatformRestRequestAsync> _request;
        readonly Mock<IMapper> _mapper;

        const string token = "123abc";
        const string programName = "I'm in";
        const int programId = 3;
        private readonly IProgramRepository _fixture;

        public ProgramRepositoryTest()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _restRequestBuilder = new Mock<IMinistryPlatformRestRequestBuilderFactory>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);
            _restRequest = new Mock<IMinistryPlatformRestRequestBuilder>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _request = new Mock<IMinistryPlatformRestRequestAsync>();

            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(It.IsAny<string>())).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.BuildAsync()).Returns(_request.Object);

            _fixture = new ProgramRepository(_restRequestBuilder.Object,
                _apiUserRepository.Object,
                _configurationWrapper.Object,
                _mapper.Object);
        }

        [Fact]
        public void GetProgramByName()
        {
            var mockProgram = new MpProgram()
            {
                ProgramName = programName
            };
            _request.Setup(m => m.Search<MpProgram>()).Returns(Task.FromResult(new List<MpProgram>() { mockProgram }));

            var result = _fixture.GetProgramByName(programName).Result;

            Assert.NotNull(result);
            Assert.Equal(result.ProgramName, programName);
        }

        [Fact]
        public void GetProgramByNameEmpty()
        {
            // return empty list
            _request.Setup(m => m.Search<MpProgram>()).Returns(Task.FromResult(new List<MpProgram>() {}));

            var result = _fixture.GetProgramByName(programName).Result;

            Assert.Null(result);
        }
    }
}
