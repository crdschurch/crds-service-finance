using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using log4net;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;

namespace MinistryPlatform.Repositories
{
    public class ProgramRepository : MinistryPlatformBase, IProgramRepository
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProgramRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public async Task<MpProgram> GetProgramByName(string programName)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            // replace ' with '' so that we can search for
            //  a program like I'm in
            var escapedName = programName.Replace("'", "''");
            var filter = $"Program_Name = '{escapedName}'";
            var programs = await MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .BuildAsync()
                                .Search<MpProgram>();

            if(!programs.Any())
            {
                _logger.Error($"GetProgramByName: No program found with name {filter}");
                return null;
            }

            return programs.First();
        }
    }
}
