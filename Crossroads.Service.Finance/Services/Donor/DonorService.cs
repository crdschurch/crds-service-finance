using System;
using System.Threading.Tasks;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Donors;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Services.Donor
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository _donorRepository;
        private readonly IContactRepository _contactRepository;
        private readonly int _mpDefaultContactDonorId, _mpNotSiteSpecificCongregationId;
        
        public DonorService(IDonorRepository donorRepository, IConfigurationWrapper configurationWrapper, IContactRepository contactRepository)
        {
            _donorRepository = donorRepository;
            _contactRepository = contactRepository;
            _mpNotSiteSpecificCongregationId = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "NotSiteSpecific") ?? 5;
            _mpDefaultContactDonorId = configurationWrapper.GetMpConfigIntValue("COMMON", "defaultDonorID") ?? 1;
        }
        
        public async Task<MpDonor> CreateDonor(MpDonor donor)
        {
            return await _donorRepository.CreateDonor(donor);
        }
        
        public async Task<MpDonor> CreateDonor(PushpayTransactionBaseDto gift)
        {
            MpDonor mpDonor;
            var matchedContact = await _contactRepository.MatchContact(gift.Payer.FirstName, gift.Payer.LastName,
                                            gift.Payer.MobileNumber, gift.Payer.EmailAddress);

            if (matchedContact != null)
            {
                // contact was matched
                if (matchedContact.DonorId != null) return matchedContact;
                
                // matched contact did not have a donor record,
                //   so create and attach donor to contact
                mpDonor = new MpDonor()
                {
                    ContactId = matchedContact.ContactId,
                    StatementFrequencyId = 1, // quarterly
                    StatementTypeId = 1, // individual
                    StatementMethodId = 2, // email+online
                    SetupDate = DateTime.Now
                };
                matchedContact.DonorId = (await CreateDonor(mpDonor)).DonorId;

                return matchedContact;
            }

            mpDonor = new MpDonor()
            {
                DonorId = _mpDefaultContactDonorId,
                CongregationId = _mpNotSiteSpecificCongregationId
            };
            return mpDonor;
        }
        
        public async Task<int?> FindDonorId(PushpayTransactionBaseDto pushpayTransactionBaseDto)
        {
            return await _donorRepository.GetDonorIdByProcessorId(pushpayTransactionBaseDto.Payer.Key);
        }

        //public Task<MpDonorAccount> GetOrCreateDonorAccount(PushpayTransactionBaseDto basePushPayTransaction, int donorId)
        //{
	       // var mpDonorAccount = MapDonorAccountPaymentDetails(basePushPayTransaction, donorId);
	       // var mpCurrentDonorAccounts = await _donationService.GetDonorAccounts(donorId);

	       // var matchingDonorAccounts = mpCurrentDonorAccounts.Where(da =>
		      //  da.Closed == mpDonorAccount.Closed
		      //  && da.NonAssignable == mpDonorAccount.NonAssignable
		      //  && da.AccountNumber == mpDonorAccount.AccountNumber
		      //  && da.DonorId == mpDonorAccount.DonorId
		      //  && da.InstitutionName == mpDonorAccount.InstitutionName
		      //  && da.ProcessorId == mpDonorAccount.ProcessorId
		      //  && da.RoutingNumber == mpDonorAccount.RoutingNumber
		      //  && da.AccountTypeId == mpDonorAccount.AccountTypeId
		      //  && da.ProcessorTypeId == mpDonorAccount.ProcessorTypeId).ToList();

	       // if (matchingDonorAccounts.Count > 0)
	       // {
		      //  return matchingDonorAccounts[0];
	       // }
	       // else
	       // {
		      //  return
        //}
    }
}