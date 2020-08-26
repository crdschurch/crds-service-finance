using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Models;

namespace MinistryPlatform.DonorAccounts
{
	public interface IDonorAccountRepository
	{
		Task<List<MpDonorAccount>> GetDonorAccounts(int donorId);
		Task<MpDonorAccount> CreateDonorAccount(MpDonorAccount donor);
	}
}
