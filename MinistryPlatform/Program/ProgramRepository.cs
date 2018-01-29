using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;

namespace MinistryPlatform.Repositories
{
    public class ProgramRepository : MinistryPlatformBase, IProgramRepository
    {
        public ProgramRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public MpProgram GetProgramByName(string programName)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            var filter = $"Program_Name LIKE '{programName}'";
            var programs = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpProgram>();

            if(!programs.Any())
            {
                return null;
            }

            return programs.First();
        }
    }
}
