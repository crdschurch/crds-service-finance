using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Services.Congregations
{
	public interface ICongregationService
	{
		Task<int> LookupCongregationId(List<PushpayFieldValueDto> pushpayFields, string campusKey);
	}
}
