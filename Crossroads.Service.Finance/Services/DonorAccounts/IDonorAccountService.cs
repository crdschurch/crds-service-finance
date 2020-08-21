using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinistryPlatform.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Services.DonorAccounts
{
	public interface IDonorAccountService
	{
		Task<MpDonorAccount> GetOrCreateDonorAccount(PushpayTransactionBaseDto basePushPayTransaction,
			int donorId);
	}
}
