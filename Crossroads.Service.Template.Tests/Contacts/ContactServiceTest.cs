using System;
using AutoMapper;
using Crossroads.Service.Template.Models;
using Crossroads.Service.Template.Services.Contact;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Mock;
using Moq;
using Xunit;

namespace Crossroads.Service.Template.Test.Events
{
    public class ContactServiceTest
    {
        readonly Mock<IContactRepository> _contactRepository;
        readonly Mock<IMapper> _mapper;
        readonly ContactService _contactService;

        public ContactServiceTest()
        {
            _contactRepository = new Mock<IContactRepository>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _contactService = new ContactService(_contactRepository.Object, _mapper.Object);
        }


        [Theory]
        [InlineData(222)]
        public void GetContact(int contactId)
        {
            _contactRepository.Setup(m => m.GetContact(222))
                  .Returns(MpContactMock.CreateList(555, 666)[0]);
            _mapper.Setup(m => m.Map<ContactDto>(It.IsAny<MpContact>())).Returns(ContactMock.Create());
            var result = _contactService.GetContact(contactId);
            Assert.Equal(22, result.ContactId);
        }

        [Theory]
        [InlineData(223)]
        public void GetContactEmpty(int contactId)
        {
            _contactRepository.Setup(m => m.GetContact(223))
                              .Throws(new Exception());
            _mapper.Setup(m => m.Map<ContactDto>(It.IsAny<MpContact>())).Returns(ContactMock.CreateEmpty());
            Assert.Throws<Exception>(() => _contactService.GetContact(contactId));
        }

    }
}
