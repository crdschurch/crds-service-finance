using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Repositories
{
    public class ContactRepository : MinistryPlatformBase, IContactRepository
    {
        public ContactRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public MpDonor MatchContact(string firstName, string lastName, string phone, string email)
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
                            .ExecuteStoredProc<MpDonor>("api_Common_FindMatchingContact", parameters);
                         
            if(!result.Any())
            {
                return null;
            }

            var donorContact = result.First().First();
            // this proc puts Donor_ID on Donor_Record field, so lets reassign
            donorContact.DonorId = donorContact.DonorRecord;

            return donorContact;
        }

        public MpHousehold GetHousehold(int householdId)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();
            var columns = new string[] {
                "Household_ID",
                "Congregation_ID"
            };

            var filter = $"Household_ID = {householdId}";
            var households = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithSelectColumns(columns)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpHousehold>();

            if (!households.Any())
            {
                return null;
            }

            return households.First();
        }

        public void UpdateProcessor(int donorId, string processorId)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            var fields = new JObject(
                new JProperty("Donor_ID", donorId),
                new JProperty("Processor_ID", processorId)
            );

            MpRestBuilder.NewRequestBuilder()
                         .WithAuthenticationToken(token)
                         .Build()
                         .Update(fields, "Donors");
        }

        public MpDonor FindDonorByProcessorId(string processorId)
        {
            var token = ApiUserRepository.GetDefaultApiUserToken();

            var columns = new string[] {
                "Donors.[Donor_ID]",
                "Contact_ID_Table.[Contact_ID]",
                "Contact_ID_Table_Household_ID_Table.[Household_ID]"
            };

            var filter = $"Processor_ID = '{processorId}'";
            var donors = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithSelectColumns(columns)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpDonor>();

            if (!donors.Any())
            {
                return null;
            }

            return donors.First();
        }
    }
}
