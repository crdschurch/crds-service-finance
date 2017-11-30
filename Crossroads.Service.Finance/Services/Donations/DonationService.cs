using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Donations;
using MinistryPlatform.Models;

namespace Crossroads.Service.Finance.Services.Donations
{
    public class DonationService
    {
        private readonly MinistryPlatform.Donations.IDonationRepository _donationRepository;

        public DonationService(IDonationRepository donationRepository)
        {
            _donationRepository = donationRepository;
        }

        public DepositDto GetDepositByProcessorTransferId(string key)
        {
            return (Mapper.Map<MpDeposit, DepositDto>(_donationRepository.GetDepositByProcessorTransferId(key)));
        }

        public DonationDto GetDonationByTransactionCode(string transactionCode)
        {
            return (Mapper.Map<MpDonation, DonationDto>(_donationRepository.GetDonationByTransactionCode(transactionCode)));
        }
    }
}
