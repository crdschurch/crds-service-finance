using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinistryPlatform.DonorAccounts;
using MinistryPlatform.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Services.DonorAccounts
{
	public class DonorAccountService : IDonorAccountService
	{
		private readonly IDonorAccountRepository _donorAccountRepository;

		private const int PushpayProcessorTypeId = 1;

		public DonorAccountService(IDonorAccountRepository donorAccountRepository)
		{
			_donorAccountRepository = donorAccountRepository;
		}

		public async Task<MpDonorAccount> GetOrCreateDonorAccount(PushpayTransactionBaseDto basePushPayTransaction,
			int donorId)
		{
			var mpDonorAccount = MapDonorAccountPaymentDetails(basePushPayTransaction, donorId);
			var mpCurrentDonorAccounts = await _donorAccountRepository.GetDonorAccounts(donorId);

			var matchingDonorAccounts = mpCurrentDonorAccounts.Where(da =>
				da.Closed == mpDonorAccount.Closed
				&& da.NonAssignable == mpDonorAccount.NonAssignable
				&& da.AccountNumber == mpDonorAccount.AccountNumber
				&& da.DonorId == mpDonorAccount.DonorId
				&& da.InstitutionName == mpDonorAccount.InstitutionName
				&& da.ProcessorId == mpDonorAccount.ProcessorId
				&& da.RoutingNumber == mpDonorAccount.RoutingNumber
				&& da.AccountTypeId == mpDonorAccount.AccountTypeId
				&& da.ProcessorTypeId == mpDonorAccount.ProcessorTypeId).ToList();

			if (matchingDonorAccounts.Count > 0)
			{
				return matchingDonorAccounts[0];
			}
			else
			{
				return await _donorAccountRepository.CreateDonorAccount(mpDonorAccount);
			}
		}

		private MpDonorAccount MapDonorAccountPaymentDetails(PushpayTransactionBaseDto basePushpayTransaction, int? donorId = null)
		{
			var isBank = basePushpayTransaction.PaymentMethodType.ToLower() == "ach";
			var mpDonorAccount = new MpDonorAccount()
			{
				AccountNumber = isBank ? basePushpayTransaction.Account.Reference : basePushpayTransaction.Card.Reference,
				InstitutionName = isBank ? basePushpayTransaction.Account.BankName : Helpers.Helpers.GetCardBrand(basePushpayTransaction.Card.Brand),
				RoutingNumber = isBank ? basePushpayTransaction.Account.RoutingNumber : null,
				NonAssignable = false,
				DomainId = 1,
				Closed = false,
				ProcessorId = basePushpayTransaction.Payer.Key,
				ProcessorTypeId = PushpayProcessorTypeId
			};
			if (donorId != null)
			{
				mpDonorAccount.DonorId = donorId.Value;
			}
			// set account type
			switch (basePushpayTransaction.PaymentMethodType.ToLower())
			{
				case "ach":
					if (basePushpayTransaction.Account.AccountType == "Checking")
					{
						mpDonorAccount.AccountTypeId = MpAccountTypes.Checkings;
					}
					else if (basePushpayTransaction.Account.AccountType == "Savings")
					{
						mpDonorAccount.AccountTypeId = MpAccountTypes.Savings;
					}
					break;
				case "creditcard":
					mpDonorAccount.AccountTypeId = MpAccountTypes.CreditCard;
					break;
			}
			return mpDonorAccount;
		}
	}
}

