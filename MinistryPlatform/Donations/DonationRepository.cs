using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
    public class DonationRepository : MinistryPlatformBase, IDonationRepository
    {

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DonationRepository(IMinistryPlatformRestRequestBuilderFactory builder,
            IApiUserRepository apiUserRepository,
            IConfigurationWrapper configurationWrapper,
            IMapper mapper) : base(builder, apiUserRepository, configurationWrapper, mapper) { }

        public MpDonation GetDonationByTransactionCode(string transactionCode)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();

            var filter = $"Transaction_Code = '{transactionCode}'";
            var donations = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpDonation>();

            if(!donations.Any())
            {
                // TODO possibly refactor to create a more custom exception
                //throw new Exception($"Donation does not exist for transaction code: {transactionCode}");
                _logger.Error($"Donation does not exist for transaction code: {transactionCode}");
                return null;
            }

            return donations.First();
        }

        public List<MpDonation> Update(List<MpDonation> donations)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();
            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Update(donations);
        }

        public MpDonation Update(MpDonation donation)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();
            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Update(donation);
        }

        public MpDonor CreateDonor(MpDonor donor)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();
            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Create(donor);
        }

        public MpDonorAccount CreateDonorAccount(MpDonorAccount donor)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();
            return MpRestBuilder.NewRequestBuilder()
                .WithAuthenticationToken(token)
                .Build()
                .Create(donor);
        }

        public void UpdateDonorAccount(JObject donorAccount)
        {
            var token = ApiUserRepository.GetDefaultApiClientToken();

            try
            {
                MpRestBuilder.NewRequestBuilder()
                    .WithAuthenticationToken(token)
                    .Build()
                    .Update(donorAccount, "Donor_Accounts");
            }
            catch (Exception e)
            {
                _logger.Error($"UpdateRecurringGift: Error updating recurring gift: {JsonConvert.SerializeObject(donorAccount)}", e);
            }
        }

        public MpContactDonor GetContactDonor(int contactId)
        {
            MpContactDonor donor;
            try
            {
                var token = ApiUserRepository.GetDefaultApiClientToken();
                var parameters = new Dictionary<string, object>
                {
                    { "@Contact_ID", contactId }
                };

                var filter = $"api_crds_Get_Contact_Donor";
                var storedProcReturn = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpContactDonor>();

                if (storedProcReturn != null && storedProcReturn.Count > 0)
                    donor = storedProcReturn[0];
                else
                {
                    donor = new MpContactDonor
                    {
                        ContactId = contactId,
                        RegisteredUser = true
                    };
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    string.Format("GetDonorRecord failed.  Contact Id: {0}", contactId), ex);
            }

            return donor;
        }

        public List<MpRecurringGift> GetRecurringGifts(string token)
        {
            var filter = $"Transaction_Code = '123'";
            var recurringGifts = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpRecurringGift>();

            return recurringGifts.ToList();
        }

        public List<MpPledge> GetPledges(string token)
        {
            var filter = $"Transaction_Code = '123'";
            var pledges = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                .WithFilter(filter)
                                .Build()
                                .Search<MpPledge>();

            return pledges.ToList();
        }

        public List<MpDonation> GetDonations(string token)
        {
            //var filter = $"Transaction_Code = '123'";
            var donations = MpRestBuilder.NewRequestBuilder()
                                .WithAuthenticationToken(token)
                                //.WithFilter(filter)
                                .Build()
                                .Search<MpDonation>();
            
            return donations.ToList();
        }
    }
}
