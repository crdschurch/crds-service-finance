using AutoMapper;
using Crossroads.Service.Finance.Services.Contacts;
using MinistryPlatform.Interfaces;
using Moq;
using Xunit;

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
    }
}
