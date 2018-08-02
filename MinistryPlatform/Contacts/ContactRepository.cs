using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using Crossroads.Web.Common.Security;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Repositories
{
    public class ContactRepository : MinistryPlatformBase, IContactRepository
    {
        private readonly IDonationRepository _mpDonationRepository;
        IAuthenticationRepository _authRepo;
        private const int pushpayProcessorType = 1;

        public ContactRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper,
            IAuthenticationRepository authenticationRepository) : base(builder, apiUserRepository, configurationWrapper, mapper)
        {
            _authRepo = authenticationRepository;
        }

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
            var token = ApiUserRepository.GetDefaultApiClientToken();
            var columns = new string[] {
                "Congregation_ID"
            };

            return MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithSelectColumns(columns)
                                .Build()
                                .Get<MpHousehold>(householdId);
        }

        public void UpdateProcessor(int donorId, string processorId)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();

            var fields = new JObject(
                new JProperty("Donor_ID", donorId),
                new JProperty("Processor_ID", processorId)
            );

            MpRestBuilder.NewRequestBuilder()
                         .WithAuthenticationToken(token)
                         .Build()
                         .Update(fields, "Donors");
        }

        public int GetBySessionId(string sessionId)
        {
            return _authRepo.GetContactId(sessionId);
        }

        public MpContact GetContact(int contactId)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();
            var columns = new string[] {
                "Contact_ID",
                "Email_Address",
                "First_Name",
                "Mobile_Phone",
                "Last_Name",
                "Date_of_Birth",
                "Participant_Record",
                "Nickname"
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

        public List<MpContactRelationship> GetContactRelationships(int contactId, int contactRelationshipId)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");
            var columns = new string[] {
                "Contact_Relationship_ID",
                "Contact_ID_Table.[Contact_ID]",
                "Relationship_ID_Table.[Relationship_ID]",
                "Related_Contact_ID_Table.[Contact_ID] AS [Related_Contact_ID]",
                "Start_Date",
                "End_Date",
                "Notes"
            };

            var filters = new string[]
            {
                $"Contact_ID_Table.[Contact_ID] = {contactId}",
                $"Relationship_ID_Table.[Relationship_ID] = {contactRelationshipId}",
                $"[Start_Date] <= '{DateTime.Now:yyyy-MM-dd}'",
                $"([End_Date] IS NULL OR [End_Date] > '{DateTime.Now:yyyy-MM-dd}')"
            };

            var relatedContacts = MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .WithSelectColumns(columns)
                .WithFilter(String.Join(" AND ", filters))
                .Build()
                .Search<MpContactRelationship>();

            return relatedContacts;
        }
    }
}
