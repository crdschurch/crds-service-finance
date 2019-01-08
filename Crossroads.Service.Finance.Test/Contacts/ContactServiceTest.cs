using System.Collections.Generic;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Services;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using MinistryPlatform.Users;
using Moq;
using Xunit;
using Mock;

namespace Crossroads.Service.Finance.Test.Contacts
{
    public class ContactServiceTest
    {
        private readonly Mock<IContactRepository> _contactRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IMapper> _mapper;

        private readonly IContactService _fixture;

        // TODO: This functionality should be moved into middleware to avoid having to explicitly call it in 
        // service layer functions
        public ContactServiceTest()
        {
            _contactRepository = new Mock<IContactRepository>();
            _userRepository = new Mock<IUserRepository>();
            _mapper = new Mock<IMapper>();
            _fixture = new ContactService(_contactRepository.Object, _userRepository.Object, _mapper.Object);
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

            _contactRepository.Setup(m => m.GetActiveContactRelationships(5544555, cogiverRelationshipId)).Returns(mpContactRelationships);
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
            var mockContactId = 5565;
            _contactRepository.Setup(m => m.GetContact(It.IsAny<int>())).Returns(MpContactMock.Create());
            _mapper.Setup(m => m.Map<ContactDto>(It.IsAny<MpContact>())).Returns(ContactMock.Create());
            _contactRepository.Setup(m => m.GetActiveContactRelationships(mockContactId, 42)).Returns(MpContactRelationshipMock.CreateList());
            // doesn't matter because this gets mapped
            _contactRepository.Setup(m => m.GetContact(It.IsAny<int>())).Returns(MpContactMock.Create());
            _mapper.Setup(m => m.Map<List<ContactDto>>(It.IsAny<List<MpContact>>())).Returns(ContactMock.CreateList());
            // doesn't matter because this gets mapped
            _contactRepository.Setup(m => m.GetHouseholdMinorChildren(It.IsAny<int>())).Returns(MpContactMock.CreateList());
            _mapper.Setup(m => m.Map<List<ContactDto>>(It.IsAny<List<MpContact>>())).Returns(ContactMock.CreateList());

            var result = _fixture.GetDonorRelatedContacts(mockContactId);

            _contactRepository.VerifyAll();

            Assert.Equal(5, result.Count);
            // ensure ordered by logged in user, then nickname/first name/last name
            Assert.Equal("Charles", result[0].Nickname);
            Assert.Equal("Bob", result[1].Nickname);
            Assert.Equal("Bob", result[2].Nickname);
            Assert.Equal("George", result[3].Nickname);
            Assert.Equal("George", result[4].Nickname);
        }

        [Fact]
        public void ShouldGetAddressByContactID()
        {
            // Arrange
            var contactId = 5544555;
            var contactAddressDto = new ContactAddressDto();

            _contactRepository.Setup(m => m.GetContactAddressByContactId(contactId)).Returns(new MpContactAddress());
            _mapper.Setup(m => m.Map<ContactAddressDto>(It.IsAny<MpContactAddress>())).Returns(contactAddressDto);

            // Act
            var result = _fixture.GetContactAddressByContactId(contactId);

            // Assert
            Assert.NotNull(result);
        }
    }
}
