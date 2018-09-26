﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Crossroads.Web.Common.Configuration;
using Crossroads.Web.Common.MinistryPlatform;
using log4net;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Repositories
{
    public class RecurringGiftRepository : MinistryPlatformBase, IRecurringGiftRepository
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public RecurringGiftRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }


        public MpRecurringGift FindRecurringGiftBySubscriptionId(string subscriptionId)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var filter = $"Subscription_ID = '{subscriptionId}'";
            var gifts = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpRecurringGift>();

            if (!gifts.Any())
            {
                throw new Exception($"Recurring Gift does not exist for subscription id: {subscriptionId}");
            }

            return gifts.First();
        }

        public MpRecurringGift CreateRecurringGift(MpRecurringGift mpRecurringGift)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            try
            {
                return MpRestBuilder.NewRequestBuilder()
                    .WithAuthenticationToken(token)
                    .Build()
                    .Create(mpRecurringGift);
            }
            catch (Exception e)
            {
                _logger.Error($"CreateRecurringGift: Error creating recurring gift: {JsonConvert.SerializeObject(mpRecurringGift)}", e);
                return null;
            }
        }

        public void UpdateRecurringGift(JObject mpRecurringGift)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            try
            {
                MpRestBuilder.NewRequestBuilder()
                    .WithAuthenticationToken(token)
                    .Build()
                    .Update(mpRecurringGift, "Recurring_Gifts");
            }
            catch (Exception e)
            {
                _logger.Error($"UpdateRecurringGift: Error updating recurring gift: {JsonConvert.SerializeObject(mpRecurringGift)}", e);
            }
        }

        public List<MpRecurringGift> FindRecurringGiftsByDonorId(int donorId)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var columns = new string[] {
                "Recurring_Gifts.[Recurring_Gift_ID]",
                "Donor_ID_Table_Contact_ID_Table.[Contact_ID]",
                "Donor_ID_Table.[Donor_ID]",
                "Donor_Account_ID_Table.[Donor_Account_ID]",
                "Frequency_ID_Table.[Frequency_ID]",
                "Recurring_Gifts.[Day_Of_Month]",
                "Day_Of_Week_ID_Table.[Day_Of_Week_ID]",
                "Recurring_Gifts.[Amount]",
                "Recurring_Gifts.[Start_Date]",
                "Recurring_Gifts.[End_Date]",
                "Program_ID_Table.[Program_ID]",
                "Program_ID_Table.[Program_Name]",
                "Congregation_ID_Table.[Congregation_ID]",
                "Recurring_Gifts.[Subscription_ID]",
                "Recurring_Gifts.[Consecutive_Failure_Count]",
                "Recurring_Gifts.[Source_Url]",
                "Recurring_Gifts.[Predefined_Amount]",
                "Recurring_Gifts.[Vendor_Detail_Url]",
                "Recurring_Gift_Status_ID_Table.[Recurring_Gift_Status]",
                "Recurring_Gift_Status_ID_Table.[Recurring_Gift_Status_ID]"
            };
            var filter = $"Donor_ID_Table.[Donor_ID] = '{donorId}'";
            var gifts = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithSelectColumns(columns)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpRecurringGift>();

            return gifts;
        }

	    public List<MpRecurringGift> FindRecurringGiftsBySubscriptionIds(List<string> subscriptionIds)
        {
            var token = ApiUserRepository.GetApiClientToken("CRDS.Service.Finance");

            var filter = $"Subscription_ID IN ({string.Join(",", subscriptionIds.Select(item => "'" + item + "'"))})";
            var gifts = MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .WithFilter(filter)
                .Build()
                .Search<MpRecurringGift>();

            return gifts;
        }
    }
}
