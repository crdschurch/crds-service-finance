using System.Collections.Generic;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Services;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Moq;
using Xunit;
using Mock;

namespace Crossroads.Service.Finance.Test.Contacts
{
    public class ContactServiceTest
    {
        private readonly Mock<IContactRepository> _contactRepository;
        private readonly Mock<IMapper> _mapper;

        private readonly IContactService _fixture;

        // TODO: This functionality should be moved into middleware to avoid having to explicitly call it in 
        // service layer functions
        public ContactServiceTest()
        {
            _contactRepository = new Mock<IContactRepository>();
            _mapper = new Mock<IMapper>();
            _fixture = new ContactService(_contactRepository.Object, _mapper.Object);
        }

        [Fact]
        public void ShouldGetContactIdByToken()
        {
            // Arrange
            var token = "123abc";
            var contactId = 1234567;

            _contactRepository.Setup(m => m.GetBySessionId(token)).Returns(contactId);

            // Act
            var result = _fixture.GetContactIdBySessionId(token);

            // Assert
            Assert.Equal(1234567, result);
        }

        [Fact]
        public void ShouldGetCogivers()
        {
            // Arrange
            var contactId = 5544555;
            var cogiverRelationshipId = 42;

            var mpContactRelationships = new List<MpContactRelationship>
            {
                new MpContactRelationship
                {
                    ContactId = 6778899,
                    RelatedContactId = 9988776
                }
            };

            var mpContact = new MpContact();

            var contactDtos = new List<ContactDto>
            {
                new ContactDto()
            };

            _contactRepository.Setup(m => m.GetContactRelationships(5544555, cogiverRelationshipId)).Returns(mpContactRelationships);
            _contactRepository.Setup(m => m.GetContact(9988776)).Returns(mpContact);
            _mapper.Setup(m => m.Map<List<ContactDto>>(It.IsAny<List<MpContact>>())).Returns(contactDtos);

            // Act
            var result = _fixture.GetCogiversByContactId(contactId);

            // Assert
            Assert.NotNull(result);
            _contactRepository.VerifyAll();
        }

        [Fact]
        public void ShouldGetDonorRelatedContacts()
        {
            var token = "token-567";
            var mockContactId = 5565;
            _contactRepository.Setup(m => m.GetBySessionId(token)).Returns(mockContactId);
            _contactRepository.Setup(m => m.GetContact(mockContactId)).Returns(MpContactMock.Create());
            _mapper.Setup(m => m.Map<ContactDto>(It.IsAny<MpContact>())).Returns(ContactMock.Create());
            _contactRepository.Setup(m => m.GetContactRelationships(mockContactId, 2)).Returns(MpContactRelationshipMock.CreateList());
            // doesn't matter because this gets mapped
            _contactRepository.Setup(m => m.GetContact(It.IsAny<int>())).Returns(MpContactMock.Create());
            _mapper.Map<List<ContactDto>>(cogivers);
            _contactRepository.Setup(m => m.GetHouseholdMinorChildren(It.IsAny<int>())).Returns(MpContactMock.CreateList());
            _mapper.Map<List<ContactDto>>(householdMinorChildren);

            var result = _fixture.GetDonorRelatedContacts(token);
        }
    }
}
