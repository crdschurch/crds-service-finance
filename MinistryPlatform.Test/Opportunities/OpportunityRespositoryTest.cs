using System;
using System.Collections.Generic;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Models;
using MinistryPlatform.Repositories;
using Mock;
using Moq;
using Xunit;

namespace MinistryPlatform.Test.Opportunties
{
    public class OpportunityRepositoryTest
    {
        readonly OpportunityRepository _fixture;
        readonly Mock<IApiUserRepository> _apiUserRepository;
        readonly Mock<IConfigurationWrapper> _configurationWrapper;
        readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        readonly Mock<IMinistryPlatformRestRequest> _request;
        readonly Mock<IMapper> _mapper;
        const int opportunityId = 55578;
        const string token = "token-345";

        public OpportunityRepositoryTest()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _restRequestBuilder = new Mock<IMinistryPlatformRestRequestBuilderFactory>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);
            _restRequest = new Mock<IMinistryPlatformRestRequestBuilder>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _request = new Mock<IMinistryPlatformRestRequest>();
            _fixture = new OpportunityRepository(_restRequestBuilder.Object,
                                             _apiUserRepository.Object,
                                             _configurationWrapper.Object,
                                             _mapper.Object);
        }

        private void Setup() {
            var selectColumns = new string[] {
                "Opportunity_ID",
                "Opportunity_Title",
                "Add_to_Group as [Group_ID]",
                "Shift_Start",
                "Shift_End",
                "Room"
            };
            var filter = $"Opportunity_ID = {opportunityId}";
            _apiUserRepository.Setup(r => r.GetDefaultApiUserToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);
        }

        [Fact]
        public void GetOpportunity()
        {
            Setup();
            _request.Setup(m => m.Search<MpOpportunity>()).Returns(new List<MpOpportunity>() { MpOpportunityMock.Create(opportunityId) });

            var responseOpportunity = _fixture.GetOpportunity(opportunityId);

            Assert.Equal(opportunityId, responseOpportunity.OpportunityId);
        }

        [Fact]
        public void GetOpportunityEmpty(){
            Setup();
            _request.Setup(m => m.Search<MpOpportunity>()).Returns(new List<MpOpportunity>());

            Assert.Throws<Exception>(() => _fixture.GetOpportunity(opportunityId));
        }
    }
}
