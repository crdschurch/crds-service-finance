using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Repositories;
using MinistryPlatform.Models;
using Moq;
using Xunit;
using Newtonsoft.Json.Linq;
using System.Linq;
using Mock;

namespace MinistryPlatform.Test.Pledges
{
    public class PledgeRepositoryTest
    {
        readonly Mock<IApiUserRepository> _apiUserRepository;
        readonly Mock<IConfigurationWrapper> _configurationWrapper;
        readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        readonly Mock<IMinistryPlatformRestRequest> _request;
        readonly Mock<IMapper> _mapper;

        private string token = "123abc";
        const int contactId = 7344;

        private readonly IPledgeRepository _fixture;

        public PledgeRepositoryTest()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _restRequestBuilder = new Mock<IMinistryPlatformRestRequestBuilderFactory>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);
            _restRequest = new Mock<IMinistryPlatformRestRequestBuilder>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _request = new Mock<IMinistryPlatformRestRequest>(MockBehavior.Strict);

            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _fixture = new PledgeRepository(_restRequestBuilder.Object,
                _apiUserRepository.Object,
                _configurationWrapper.Object,
                _mapper.Object);
        }

        [Fact]
        public void GetActiveAndCompleted()
        {
            var contactId = 423534;
            // Arrange
            var selectColumns = new string[] {
                "Pledge_Status_ID_Table.[Pledge_Status_ID]",
                "Pledges.[Pledge_ID]",
                "Pledges.[Total_Pledge]",
                "Pledge_Campaign_ID_Table.[Pledge_Campaign_ID]",
                "Pledge_Campaign_ID_Table.[Campaign_Name]",
                "Pledge_Campaign_ID_Table_Pledge_Campaign_Type_ID_Table.[Pledge_Campaign_Type_ID]",
                "Pledge_Campaign_ID_Table.[Start_Date] as [Campaign_Start_Date]",
                "Pledge_Campaign_ID_Table.[End_Date] as [Campaign_End_Date]",
                "Donor_ID_Table_Contact_ID_Table.[Contact_ID]",
                "Pledges.[First_Installment_Date]"
            };
            var filter = $"Pledge_Status_ID_Table.[Pledge_Status_ID] IN (1,2)";
            filter += $" AND Donor_ID_Table_Contact_ID_Table.[Contact_ID] = {contactId}";
            filter += $" AND Pledge_Campaign_ID_Table_Pledge_Campaign_Type_ID_Table.[Pledge_Campaign_Type_ID] = 1";
            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.OrderBy("Pledge_Campaign_ID_Table.[Start_Date] DESC")).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpPledge>()).Returns(MpPledgeMock.CreateList());

            // Act
            var responseRecurringGift = _fixture.GetActiveAndCompleted(contactId);

            // Assert
            Assert.Equal(3, responseRecurringGift.Count);
        }
    }
}
