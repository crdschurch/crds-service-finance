﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        IAuthenticationRepository _authRepo;
        private const int HouseholdPositionMinorChild = 2;

        public ContactRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper,
            IAuthenticationRepository authenticationRepository) : base(builder, apiUserRepository, configurationWrapper, mapper)
        {
            _authRepo = authenticationRepository;
        }

        public async Task<MpDonor> MatchContact(string firstName, string lastName, string phone, string email)
        {
            var token = await ApiUserRepository.GetApiClientTokenAsync("CRDS.Service.Finance");

            var parameters = new Dictionary<string, object>
            {
                {"@FirstName", firstName},
                {"@LastName", lastName},
                {"@Phone", phone},
                {"@EmailAddress", email},
                {"@RequireEmail", email != null && email.Length > 0},
                {"@DomainId", 1},
            };

            var result = await MpRestBuilder.NewRequestBuilder()
                            .WithAuthenticationToken(token)
                            .BuildAsync()
                            .ExecuteStoredProc<MpDonor>("api_Common_FindMatchingContact", parameters);
                         
            if(!result.Any() || !result.First().Any())
            {
                return null;
            }

            var donorContact = result.First().First();
            // this proc puts Donor_ID on Donor_Record field, so lets reassign
            donorContact.DonorId = donorContact.DonorRecord;

            return donorContact;
        }

        public async Task<MpHousehold> GetHousehold(int householdId)
        {
            var token = await ApiUserRepository.GetApiClientTokenAsync("CRDS.Service.Finance");
            var columns = new string[] {
                "Congregation_ID"
            };

            return await MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithSelectColumns(columns)
                                .BuildAsync()
                                .Get<MpHousehold>(householdId);
        }

        public async Task<int> GetBySessionId(string sessionId)
        {
            return await _authRepo.GetContactIdAsync(sessionId);
        }

        public async Task<MpContact> GetContact(int contactId)
        {
            var token = await ApiUserRepository.GetApiClientTokenAsync("CRDS.Service.Finance");
            var columns = new string[] {
                "Contact_ID",
                "Household_ID",
                "Email_Address",
                "First_Name",
                "Mobile_Phone",
                "Last_Name",
                "Date_of_Birth",
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

        public async Task<List<MpContactRelationship>> GetActiveContactRelationships(int contactId, int contactRelationshipId)
        {
            var token = await ApiUserRepository.GetApiClientTokenAsync("CRDS.Service.Finance");
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
                $"([End_Date] IS NULL OR [End_Date] > '{DateTime.Now:yyyy-MM-dd}')",
                "Related_Contact_ID_Table_Contact_Status_ID_Table.[Contact_Status] = 'Active'"
            };

            var relatedContacts = await MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .WithSelectColumns(columns)
                .WithFilter(String.Join(" AND ", filters))
                .BuildAsync()
                .Search<MpContactRelationship>();
            
            return relatedContacts;
        }

        public async Task<MpContactRelationship> GetActiveContactRelationship(int contactId, int relatedContactId, int contactRelationshipId)
        {
            var token = await ApiUserRepository.GetApiClientTokenAsync("CRDS.Service.Finance");

            var filters = new string[]
            {
                $"Contact_ID_Table.[Contact_ID] = {contactId}",
                $"Related_Contact_ID_Table.[Contact_ID] = {relatedContactId}",
                $"Relationship_ID_Table.[Relationship_ID] = {contactRelationshipId}",
                $"Contact_Relationships.[Start_Date] <= '{DateTime.Now:yyyy-MM-dd}'",
                $"(Contact_Relationships.[End_Date] IS NULL OR Contact_Relationships.[End_Date] > '{DateTime.Now:yyyy-MM-dd}')"
            };

            var contactRelationships = await MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .WithFilter(String.Join(" AND ", filters))
                .BuildAsync()
                .Search<MpContactRelationship>();

            return contactRelationships.FirstOrDefault();
        }

        public async Task<List<MpContact>> GetHouseholdMinorChildren(int householdId)
        {
            var token = await ApiUserRepository.GetApiClientTokenAsync("CRDS.Service.Finance");

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

            var filters = new string[]
            {
                $"Household_ID_Table.[Household_ID] = {householdId}",
                $"Household_Position_ID_Table.[Household_Position_ID] = {HouseholdPositionMinorChild}"
            };

            return await MpRestBuilder.NewRequestBuilder()
                .WithSelectColumns(columns)
                .WithAuthenticationToken(token)
                .WithFilter(String.Join(" AND ", filters))
                .BuildAsync()
                .Search<MpContact>();
        }

        public async Task<MpContactAddress> GetContactAddressByContactId(int contactId)
        {
            var token = await ApiUserRepository.GetApiClientTokenAsync("CRDS.Service.Finance");

            var columns = new string[] {
                "[Contact_ID]",
                "Household_ID_Table_Address_ID_Table.[Address_ID]",
                "Household_ID_Table_Address_ID_Table.[Address_Line_1]",
                "Household_ID_Table_Address_ID_Table.[Address_Line_2]",
                "Household_ID_Table_Address_ID_Table.[City]",
                "Household_ID_Table_Address_ID_Table.[State/Region]",
                "Household_ID_Table_Address_ID_Table.[Postal_Code]"
            };

            var filters = new string[]
            {
                $"[Contact_ID] = {contactId}"
            };

            var result = await MpRestBuilder.NewRequestBuilder()
                .WithSelectColumns(columns)
                .WithAuthenticationToken(token)
                .WithFilter(String.Join(" AND ", filters))
                .BuildAsync()
                .Search<MpContactAddress>();

            return result.FirstOrDefault();
        }
    }
}
