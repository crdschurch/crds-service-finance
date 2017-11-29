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

namespace MinistryPlatform.Test.Contacts
{
    public class ContactRepositoryTest
    {
        readonly ContactRepository _fixture;
        readonly Mock<IApiUserRepository> _apiUserRepository;
        readonly Mock<IConfigurationWrapper> _configurationWrapper;
        readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        readonly Mock<IMinistryPlatformRestRequest> _request;
        readonly Mock<IMapper> _mapper;
        const string token = "token-345";
        const int contactId = 7344;
        const int householdId = 345345;

        public ContactRepositoryTest()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _restRequestBuilder = new Mock<IMinistryPlatformRestRequestBuilderFactory>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);
            _restRequest = new Mock<IMinistryPlatformRestRequestBuilder>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _request = new Mock<IMinistryPlatformRestRequest>();
            _fixture = new ContactRepository(_restRequestBuilder.Object,
                                             _apiUserRepository.Object,
                                             _configurationWrapper.Object,
                                             _mapper.Object);
        }

        [Fact]
        public void GetContact()
        {
            var selectColumns = new string[] {
                "Contact_ID",
                "Household_ID"
            };
            var filter = "Contact_ID = 7344";
            _apiUserRepository.Setup(r => r.GetDefaultApiUserToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpContact>()).Returns(MpContactMock.CreateList(contactId, householdId));
            var responseContact = _fixture.GetContact(contactId);
            Assert.Equal(contactId, responseContact.ContactId);
        }

        [Fact]
        public void GetContactEmpty(){
            var selectColumns = new string[] {
                "Contact_ID",
                "Household_ID"
            };
            var filter = "Contact_ID = 7344";
            _apiUserRepository.Setup(r => r.GetDefaultApiUserToken()).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(selectColumns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);
            _request.Setup(m => m.Search<MpContact>()).Returns(new List<MpContact>{});

            Assert.Throws<Exception>(() => _fixture.GetContact(contactId));
        }
    }
}
