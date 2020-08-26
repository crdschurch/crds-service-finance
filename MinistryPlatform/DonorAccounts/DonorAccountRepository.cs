using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Models;

namespace MinistryPlatform.DonorAccounts
{
	public class DonorAccountRepository : MinistryPlatformBase, IDonorAccountRepository
	{
		public DonorAccountRepository(IMinistryPlatformRestRequestBuilderFactory builder,
			IApiUserRepository apiUserRepository,
			IConfigurationWrapper configurationWrapper,
			IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

		public async Task<List<MpDonorAccount>> GetDonorAccounts(int donorId)
		{
			var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");
			var columns = new[]
			{
				"Donor_Accounts.[Donor_Account_ID]"
				, "Donor_ID_Table.[Donor_ID]"
				, "Donor_Accounts.[Non-Assignable]"
				, "Account_Type_ID_Table.[Account_Type_ID]"
				, "Donor_Accounts.[Closed]"
				, "Donor_Accounts.[Institution_Name]"
				, "Donor_Accounts.[Account_Number]"
				, "Donor_Accounts.[Routing_Number]"
				, "Donor_Accounts.[Processor_ID]"
				, "Processor_Type_ID_Table.[Processor_Type_ID]"
			};
			var filter = $"Donor_ID_Table.[Donor_ID] = { donorId }";

			return await MpRestBuilder.NewRequestBuilder().WithAuthenticationToken(token).WithSelectColumns(columns)
				.WithFilter(filter).BuildAsync().Search<MpDonorAccount>();
		}

		public async Task<MpDonorAccount> CreateDonorAccount(MpDonorAccount donor)
		{
			var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");
			return await MpRestBuilder.NewRequestBuilder()
				.WithAuthenticationToken(token)
				.BuildAsync()
				.Create(donor);
		}
	}
}
