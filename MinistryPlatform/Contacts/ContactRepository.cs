using System.Collections.Generic;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Models;
using MinistryPlatform.Interfaces;
using System.Linq;
using System;
using AutoMapper;

namespace MinistryPlatform.Repositories
{
    public class ContactRepository : MinistryPlatformBase, IContactRepository
    {
        public ContactRepository(IMinistryPlatformRestRequestBuilderFactory builder,
                               IApiUserRepository apiUserRepository,
                               IConfigurationWrapper configurationWrapper,
                               IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) {}

        public MpContact GetContact(int contactId)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();
            var columns = new string[] {
                "Contact_ID",
                "Household_ID"
            };
            var filter = $"Contact_ID = {contactId}";
            var contacts = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithSelectColumns(columns)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpContact>();
            if (!contacts.Any())
            {
                throw new Exception($"No contact found for contact: {contactId}");
            }
            return contacts.FirstOrDefault();
        }
    }


}
