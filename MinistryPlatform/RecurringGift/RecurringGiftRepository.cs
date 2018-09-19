using System;
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

        public List<MpRecurringGift> FindRecurringGiftsBySubscriptionIds(List<string> subscriptionIds)
        {
            var x = string.Join(",", subscriptionIds.Select(item => "'" + item + "'"));
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
