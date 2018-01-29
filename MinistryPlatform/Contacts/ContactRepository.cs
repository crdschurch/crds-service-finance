using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Repositories
{
    public class ContactRepository : MinistryPlatformBase, IContactRepository
    {
        public ContactRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public MpContact MatchContact(string firstName, string lastName, string phone, string email)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var parameters = new Dictionary<string, object>
            {
                {"@FirstName", firstName},
                {"@LastName", lastName},
                {"@Phone", phone},
                {"@EmailAddress", email},
                {"@RequireEmail", email.Length > 0},
                {"@DomainId", 1},
            };

            var result = MpRestBuilder.NewRequestBuilder()
                            .WithAuthenticationToken(token)
                            .Build()
                            .ExecuteStoredProc<MpContact>("api_Common_FindMatchingContact", parameters);
                         
            if(!result.Any())
            {
                return null;
            }

            return result.First().First();
        }
    }
}
