using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using Crossroads.Web.Common.Security;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;

namespace MinistryPlatform.Users
{
    public class UserRepository : MinistryPlatformBase, IUserRepository
    {
        IAuthenticationRepository _authRepo;

        public UserRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper,
            IAuthenticationRepository authenticationRepository) : base(builder, apiUserRepository, configurationWrapper, mapper)
        {
            _authRepo = authenticationRepository;
        }

        public async Task<int> GetUserByEmailAddress(string emailAddress)
        {
            var token = await ApiUserRepository.GetApiClientTokenAsync("CRDS.Common");
            var columns = new string[] {
                "User_ID",
                "Contact_ID",
                "User_Email"
            };
            var filter = $"User_Email = '{emailAddress}'";
            var users = await MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .WithSelectColumns(columns)
                .WithFilter(filter)
                .BuildAsync()
                .Search<MpUser>();
            if (!users.Any())
            {
                throw new Exception($"No user found for email address: {emailAddress}");
            }
            return users.FirstOrDefault().ContactId;
        }
    }
}
