using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Helpers;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Congregations;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Services.Congregations
{
	public class CongregationService : ICongregationService
	{
		private readonly ICongregationRepository _congregationRepository;
		private readonly IConfigurationWrapper _configurationWrapper;

		private readonly string CongregationFieldKey = Environment.GetEnvironmentVariable("PUSHPAY_SITE_FIELD_KEY");
		private const int NotSiteSpecificCongregationId = 5;

		public CongregationService(ICongregationRepository congregationRepository, IConfigurationWrapper configurationWrapper)
		{
			_congregationRepository = congregationRepository;
			_configurationWrapper = configurationWrapper;
		}

		public async Task<int> LookupCongregationId(List<PushpayFieldValueDto> pushpayFields, string campusKey)
		{
			var lookupCongregationId = 0;

			if (pushpayFields != null && pushpayFields.Any(r => r.Key == CongregationFieldKey))
			{
				var congregationName = Helpers.Helpers.Translate(pushpayFields.First(r => r.Key == CongregationFieldKey).Value);

				// TODO: consider caching these values on application startup
				var congregations = await _congregationRepository.GetCongregationByCongregationName(congregationName);

				if (congregations.Any())
				{
					lookupCongregationId = congregations.First(r => r.CongregationName == congregationName).CongregationId;
				}
			}
			else
			{
				// get the pushpay campus key here
				lookupCongregationId = (await _configurationWrapper.GetMpConfigIntValueAsync("CRDS-FINANCE", campusKey)).GetValueOrDefault();
			}

			if (lookupCongregationId == 0)
			{
				lookupCongregationId = NotSiteSpecificCongregationId;
			}

			return lookupCongregationId;
		}
	}
}
