using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using Crossroads.Web.Common.Security;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using MinistryPlatform.Repositories;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MinistryPlatform.Test.Contacts
{
    public class ContactRepositoryTest
    {
        readonly Mock<IApiUserRepository> _apiUserRepository;
        readonly Mock<IConfigurationWrapper> _configurationWrapper;
        readonly Mock<IMinistryPlatformRestRequestBuilder> _restRequest;
        readonly Mock<IMinistryPlatformRestRequestBuilderFactory> _restRequestBuilder;
        readonly Mock<IMinistryPlatformRestRequest> _request;
        readonly Mock<IMapper> _mapper;
        readonly Mock<IAuthenticationRepository> _authenticationRepository;

        private readonly IContactRepository _fixture;

        private string token = "123abc";
        private string clientId = "CRDS.Service.Finance";

        public ContactRepositoryTest()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _restRequestBuilder = new Mock<IMinistryPlatformRestRequestBuilderFactory>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);
            _restRequest = new Mock<IMinistryPlatformRestRequestBuilder>(MockBehavior.Strict);
            _authenticationRepository = new Mock<IAuthenticationRepository>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
 
            _request = new Mock<IMinistryPlatformRestRequest>();

            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _fixture = new ContactRepository(_restRequestBuilder.Object,
                _apiUserRepository.Object,
                _configurationWrapper.Object,
                _mapper.Object,
                _authenticationRepository.Object);
        }

        [Fact]
        public void ShouldMatchContact()
        {
            // Arrange
            var firstName = "Billy";
            var lastName = "Hamilton";
            var phone = "513-555-5506";
            var email = "billy.test@test.com";

            var parameters = new Dictionary<string, object>
            {
                {"@FirstName", firstName},
                {"@LastName", lastName},
                {"@Phone", phone},
                {"@EmailAddress", email},
                {"@RequireEmail", email.Length > 0},
                {"@DomainId", 1},
            };

            var procResult = new List<List<MpDonor>>
            {
                new List<MpDonor>
                {
                    new MpDonor()
                }
            };

            _apiUserRepository.Setup(m => m.GetApiClientToken(clientId)).Returns(token);
            _request.Setup(m => m.ExecuteStoredProc<MpDonor>("api_Common_FindMatchingContact", parameters)).Returns(procResult);

            // Act
            var result = _fixture.MatchContact(firstName, lastName, phone, email);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldGetHousehold()
        {
            // Arrange
            var householdId = 7766888;

            var columns = new string[] {
                "Congregation_ID"
            };

            _apiUserRepository.Setup(m => m.GetApiClientToken(clientId)).Returns(token);

            _apiUserRepository.Setup(r => r.GetApiClientToken("CRDS.Service.Finance")).Returns(token);
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(columns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Get<MpHousehold>(householdId)).Returns(new MpHousehold());

            // Act
            var result = _fixture.GetHousehold(householdId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldGetContact()
        {
            // Arrange
            var contactId = 7766888;

            var mpContacts = new List<MpContact>
            {
                new MpContact()
            };

            var columns = new string[] {
                "Contact_ID",
                "Household_ID",
                "Email_Address",
                "First_Name",
                "Mobile_Phone",
                "Last_Name",
                "Date_of_Birth",
                "Participant_Record",
                "Nickname"
            };

            _apiUserRepository.Setup(m => m.GetApiClientToken(clientId)).Returns(token);

            var filter = $"Contact_ID = {contactId}";
            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(columns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(filter)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpContact>()).Returns(mpContacts);

            // Act
            var result = _fixture.GetContact(contactId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldGetCogivers()
        {
            // Arrange
            var contactId = 7766888;
            var cogiverRelationshipId = 42;

            var mpContactRelationships = new List<MpContactRelationship>
            {
                new MpContactRelationship()
            };

            var columns = new string[] {
                "Contact_Relationship_ID",
                "Contact_ID_Table.[Contact_ID]",
                "Relationship_ID_Table.[Relationship_ID]",
                "Related_Contact_ID_Table.[Contact_ID] AS [Related_Contact_ID]",
                "Start_Date",
                "End_Date",
                "Notes"
            };

            var filters = new string[]
            {
                $"Contact_ID_Table.[Contact_ID] = {contactId}",
                $"Relationship_ID_Table.[Relationship_ID] = {cogiverRelationshipId}",
                $"[Start_Date] <= '{DateTime.Now:yyyy-MM-dd}'",
                $"([End_Date] IS NULL OR [End_Date] > '{DateTime.Now:yyyy-MM-dd}')"
            };

            _apiUserRepository.Setup(m => m.GetApiClientToken(clientId)).Returns(token);

            _restRequestBuilder.Setup(m => m.NewRequestBuilder()).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithAuthenticationToken(token)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithSelectColumns(columns)).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.WithFilter(String.Join(" AND ", filters))).Returns(_restRequest.Object);
            _restRequest.Setup(m => m.Build()).Returns(_request.Object);

            _request.Setup(m => m.Search<MpContactRelationship>()).Returns(mpContactRelationships);

            // Act
            var result = _fixture.GetContactRelationships(contactId, cogiverRelationshipId);

            // Assert
            Assert.NotNull(result);
        }
    }
}
