using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using log4net;
using MinistryPlatform.Models;
using Newtonsoft.Json;

namespace MinistryPlatform.JournalEntries
{
    public class JournalEntryRepository : MinistryPlatformBase, IJournalEntryRepository
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public JournalEntryRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public List<MpJournalEntry> CreateOrUpdateMpJournalEntries(List<MpJournalEntry> mpJournalEntries)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            // TODO: validate that this will create and update - can't remember if the rest api can do that in one step
            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Update(mpJournalEntries, "cr_Journal_Entries");
        }
    }
}
