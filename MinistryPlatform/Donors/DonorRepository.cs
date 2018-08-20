using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using Crossroads.Web.Common.Security;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;

namespace MinistryPlatform.Donors
{
    public class DonorRepository : MinistryPlatformBase, IDonorRepository
    {
        IAuthenticationRepository _authRepo;
        private const int pushpayProcessorType = 1;

        public DonorRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper,
            IAuthenticationRepository authenticationRepository) : base(builder, apiUserRepository, configurationWrapper, mapper)
        {
            _authRepo = authenticationRepository;
        }

        public int? GetDonorIdByProcessorId(string processorId)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var columns = new string[] {
                "Donor_Accounts.[Donor_Account_ID]",
                "Donor_Accounts.[Donor_ID]",
                "Donor_Accounts.[Non-Assignable]",
                "Donor_Accounts.[Domain_ID]",
                "Donor_Accounts.[Account_Type_ID]",
                "Donor_Accounts.[Closed]",
                "Donor_Accounts.[Institution_Name]",
                "Donor_Accounts.[Account_Number]",
                "Donor_Accounts.[Routing_Number]",
                "Donor_Accounts.[Processor_ID]",
                "Donor_Accounts.[Processor_Type_ID]"
            };

            var filter = $"Processor_ID = '{processorId}' AND Processor_Type_ID = {pushpayProcessorType}";
            var donorAccounts = MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .WithSelectColumns(columns)
                .WithFilter(filter)
                .Build()
                .Search<MpDonorAccount>();

            //return donorId if any donor accounts
            if (donorAccounts == null || !donorAccounts.Any())
            {
                return null;
            }

            return donorAccounts.First().DonorId;
        }

        public MpDonor GetDonorByDonorId(int donorId)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var donorColumns = new string[] {
                "Donors.[Donor_ID]",
                "Contact_ID_Table.[Contact_ID]",
                "Contact_ID_Table.[Email_Address]",
                "Contact_ID_Table_Household_ID_Table.[Household_ID]"
            };

            var donorFilter = $"Donor_ID = '{donorId}'";
            var donor = MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .WithSelectColumns(donorColumns)
                .WithFilter(donorFilter)
                .Build()
                .Get<MpDonor>(donorId);

            return donor;
        }
    }
}
