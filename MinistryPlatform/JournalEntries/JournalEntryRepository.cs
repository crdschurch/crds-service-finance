using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<List<MpJournalEntry>> CreateMpJournalEntries(List<MpJournalEntry> mpJournalEntries)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            // TODO: validate that this will create and update - can't remember if the rest api can do that in one step
            var result = MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .BuildAsync()
                .Create(mpJournalEntries, "cr_Journal_Entries");

            return await result;
        }

        public async Task<List<MpJournalEntry>> GetUnexportedJournalEntries()
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var selectColumns = new string[] {
                "Journal_Entry_ID",
                "Created_Date",
                "Exported_Date",
                "GL_Account_Number",
                "Batch_ID",
                "Credit_Amount",
                "Debit_Amount",
                "Description",
                "Adjustment_Year",
                "Adjustment_Month"
            };

            var filter = "Exported_Date IS NULL";

            var result = MpRestBuilder.NewRequestBuilder()
                .WithSelectColumns(selectColumns)
                .WithAuthenticationToken(token)
                .WithFilter(filter)
                .BuildAsync()
                .Search<MpJournalEntry>();

            return await result;
        }

        public async Task UpdateJournalEntries(List<MpJournalEntry> mpJournalEntries)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            await MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .BuildAsync()
                .Update(mpJournalEntries, "cr_Journal_Entries");
        }
    }
}
