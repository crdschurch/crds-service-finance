using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services.Donor;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Newtonsoft.Json;
using Pushpay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Crossroads.Service.Finance.Services.Recurring
{
    public class RecurringService : IRecurringService
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IPushpayService _pushpayService;
        private readonly INewPushpayService _newPushpayService;
        private readonly IConfigurationWrapper _configurationWrapper;
        private readonly IDonationService _donationService;
        private readonly IMapper _mapper;
        private readonly IProgramRepository _programRepository;
        private readonly IDonorService _donorService;
        private readonly IGatewayService _gatewayService;

        private const int PausedRecurringGiftStatus = 2;
        private readonly int _mpNotSiteSpecificCongregationId;

        public RecurringService(IDepositRepository depositRepository,
            IMapper mapper,
            IPushpayService pushpayService,
            INewPushpayService newPushpayService,
            IConfigurationWrapper configurationWrapper,
            IDonationService donationService,
            IDonorService donorService,
            IRecurringGiftRepository recurringGiftRepository,
            IProgramRepository programRepository,
            IGatewayService gatewayService)
        {
            _pushpayService = pushpayService;
            _newPushpayService = newPushpayService;
            _configurationWrapper = configurationWrapper;
            _donationService = donationService;
            _donorService = donorService;
            _gatewayService = gatewayService;
            _recurringGiftRepository = recurringGiftRepository;
            _programRepository = programRepository;
            _mapper = mapper;
            _mpNotSiteSpecificCongregationId = configurationWrapper.GetMpConfigIntValue("CRDS-COMMON", "NotSiteSpecific") ?? 5;
        }

        public async Task<List<string>> SyncRecurringGifts(DateTime startDate, DateTime endDate)
        {
            var start = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var end = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            int numUpdates = 0;
            var giftIdsSynced = new List<string>();

            _logger.Info($"Starting SyncRecurringGifts at {DateTime.Now:G}");

            // get new and updated recurring gifts.
            var pushpayRecurringGifts = _pushpayService.GetRecurringGiftsByDateRange(start, end);

            _logger.Info($"Syncing {pushpayRecurringGifts.Count} gifts from pushpay");

            // next, check to see if these gifts exist in MP
            var pushpayRecurringGiftIds = pushpayRecurringGifts
                .Select(r => r.PaymentToken).ToList();
            while (pushpayRecurringGiftIds.Any())
            {
                Thread.Sleep(500);

                // if the recurring gift does not exist in MP, pull the data from Pushpay and create it
                var range = Math.Min(pushpayRecurringGiftIds.Count, 1);

                var pushpayGiftIdsToSync = pushpayRecurringGiftIds.Take(range).ToList();
                pushpayRecurringGiftIds.RemoveRange(0, range);

                var mpRecurringGifts = await
                    _recurringGiftRepository.FindRecurringGiftsBySubscriptionIds(pushpayGiftIdsToSync);

                foreach (var pushpayRecurringGiftId in pushpayGiftIdsToSync)
                {
                    var mpGift = mpRecurringGifts.FirstOrDefault(r => r.SubscriptionId == pushpayRecurringGiftId);
                    var pushPayGift = pushpayRecurringGifts.First(r => r.PaymentToken == pushpayRecurringGiftId);

                    // if the recurring gift does not exist in MP, then create it
                    if (mpGift == null)
                    {
                        _logger.Info($"Create new recurring gift: {pushpayRecurringGiftId}");

                        await _pushpayService.BuildAndCreateNewRecurringGift(pushPayGift, null);
                        giftIdsSynced.Add(pushpayRecurringGiftId);
                    }
                    // if the recurring gift DOES exist in MP, check to see when it was last updated and update it if the Pushpay version is newer
                    else
                    {
                        _logger.Info($"Comparing dates for {pushpayRecurringGiftId} ? MP: {mpGift.UpdatedOn.ToString()}, Pushpay: {pushPayGift.UpdatedOn.ToString()}");

                        if (IsPushpayDateNewer(mpGift.UpdatedOn ?? DateTime.MinValue, pushPayGift.UpdatedOn))
                        {
                            _logger.Info($"Create new recurring gift: {pushpayRecurringGiftId}");

                            await _pushpayService.UpdateRecurringGiftForSync(pushPayGift, mpGift);
                            giftIdsSynced.Add(pushpayRecurringGiftId);
                        }
                    }
                }

                numUpdates += giftIdsSynced.Count;
            }

            _logger.Info($"{numUpdates} Recurring gifts synced from Pushpay: {string.Join(", ", giftIdsSynced)}");

            return giftIdsSynced;
        }

        public async Task SyncRecurringSchedules()
        {
            int failureCount = 0;
            int? lastSyncIndex = null;
            do
            {
                var schedulesToProcess = await _recurringGiftRepository.GetUnprocessedRecurringGifts(lastSyncIndex);
                _logger.Info($"Syncing {schedulesToProcess.Count} schedules.");
                
                // MP gets the top 1000 results. So we should get the last ID so we can get the next chunk of donations
                lastSyncIndex = schedulesToProcess.Count >= 1000
                    ? schedulesToProcess.Last().RecurringGiftScheduleId
                    : (int?) null;

                while (schedulesToProcess.Any())
                {
                    // Do a chunk at a time to not overload MP
                    Thread.Sleep(500);
                    var range = Math.Min(schedulesToProcess.Count, 100);
                    var setOfSchedulesToProcess = schedulesToProcess.Take(range).ToList();
                    schedulesToProcess.RemoveRange(0, range);

                    // Sync the chunk and wait for all to finish before processing the next chunk
                    var currentResults = await Task.WhenAll(setOfSchedulesToProcess.Select(SyncSchedule).ToArray());
                    failureCount += currentResults.Where(r => !r).ToList().Count;
                }
            } while (lastSyncIndex.HasValue);

            if (failureCount > 0)
            {
                _logger.Error($"There was {failureCount} recurring schedule(s) that could not be synced.");
            }
        }

        private async Task<bool> SyncSchedule(MpRawPushPayRecurringSchedules schedule)
        {
            try
            {
                var pushPayScheduleDto = JsonConvert.DeserializeObject<PushpayRecurringGiftDto>(schedule.RawJson);
                if (!string.IsNullOrEmpty(pushPayScheduleDto.PaymentToken))
                {
                    var mpRecurringSchedule =
                        await _recurringGiftRepository.LookForRecurringGiftBySubscriptionId(
                            pushPayScheduleDto.PaymentToken);
                    if (mpRecurringSchedule != null)
                    {
                        if (IsPushpayDateNewer(mpRecurringSchedule.UpdatedOn ?? DateTime.MinValue,
                            pushPayScheduleDto.UpdatedOn))
                        {
                            await _pushpayService.UpdateRecurringGiftForSync(pushPayScheduleDto, mpRecurringSchedule);
                        }    
                    }
                    else
                    {
                        var recurringSchedule = await BuildRecurringScheduleFromPushPayData(pushPayScheduleDto);
                        await _recurringGiftRepository.CreateRecurringGift(recurringSchedule);
                        
                        // STRIPE CANCELLATION - this can be removed after there are no more Stripe recurring gifts
                        // This cancels a Stripe gift if a subscription id was uploaded to Pushpay (i.e. through pushpay migration tool)
                        if (pushPayScheduleDto.Notes != null && pushPayScheduleDto.Notes.Trim()
                            .StartsWith("sub_", StringComparison.Ordinal))
                        {
                            _gatewayService.CancelStripeRecurringGift(pushPayScheduleDto.Notes.Trim());
                        }
                        
                    }
                    await _recurringGiftRepository.FlipIsProcessedToTrue(schedule);
                }
                else
                {
                    throw new Exception("Schedule is missing payment token.");
                }
                
            }
            catch (Exception e)
            {
                _logger.Error(
                    $"Got the following error \"{e.Message}\" while processing schedule with an ID of {schedule.RecurringGiftScheduleId}");
                return false;
            }

            return true;
        }

        private bool IsPushpayDateNewer(DateTime mp, DateTime pushpay)
        {
            // MP truncates seconds from DateTime fields that are inserted/updated via
            // the MP REST API, so exclude seconds from our comparison to prevent false
            // positives.
            DateTime normalizedMp = new DateTime(mp.Year, mp.Month, mp.Day, mp.Hour, mp.Minute, 0);
            DateTime normalizedPushpay = new DateTime(pushpay.Year, pushpay.Month, pushpay.Day, pushpay.Hour, pushpay.Minute, 0);

            return normalizedPushpay > normalizedMp;
        }

        public async Task<MpRecurringGift> BuildRecurringScheduleFromPushPayData(PushpayRecurringGiftDto pushpayRecurringGift)
        {
            var mpRecurringGift = _mapper.Map<MpRecurringGift>(pushpayRecurringGift);
            var donorId = await _donorService.FindDonorId(pushpayRecurringGift);

            if (donorId.HasValue)
            {
                mpRecurringGift.DonorId = donorId.Value;
                var donorAccount = await _donationService.FindDonorAccount(pushpayRecurringGift, donorId.Value);
                mpRecurringGift.DonorAccountId =
                    donorAccount?.DonorAccountId ??
                    (await _donationService.CreateDonorAccountFromPushpay(pushpayRecurringGift, donorId.Value))
                    .DonorAccountId;
            }
            else
            {
                var donor = await _donorService.CreateDonor(pushpayRecurringGift);
                mpRecurringGift.DonorId = donor.DonorId.Value;
                mpRecurringGift.DonorAccountId =
                    (await _donationService.CreateDonorAccountFromPushpay(pushpayRecurringGift, donor.DonorId.Value))
                    .DonorAccountId;
            }

            mpRecurringGift.CongregationId = await _pushpayService.LookupCongregationId(pushpayRecurringGift.PushpayFields, pushpayRecurringGift.Campus.Key);

            if (mpRecurringGift.CongregationId == _mpNotSiteSpecificCongregationId)
            {
                _logger.Info($"No selected site for recurring gift {mpRecurringGift.RecurringGiftId}");
            }

            var programId = _newPushpayService.ParseFundIdFromExternalLinks(pushpayRecurringGift);
            mpRecurringGift.ConsecutiveFailureCount = 0;
            mpRecurringGift.ProgramId = programId ?? (await _programRepository.GetProgramByName(pushpayRecurringGift.Fund.Code)).ProgramId;
            // TODO: Make this be a part of mapping?
            var pushpayRecurringGiftStatus = _pushpayService.GetRecurringGiftStatusId(pushpayRecurringGift.Status);

            // only update the status changed date if the pushpay gift status has actually changed
            if (mpRecurringGift.RecurringGiftStatusId != pushpayRecurringGiftStatus)
            {
                mpRecurringGift.RecurringGiftStatusId = pushpayRecurringGiftStatus;
                mpRecurringGift.StatusChangedDate = DateTime.Now;
            }

            mpRecurringGift.UpdatedOn = pushpayRecurringGift.UpdatedOn;

            mpRecurringGift.EndDate =
                mpRecurringGift.RecurringGiftStatusId == MpRecurringGiftStatus.Cancelled || mpRecurringGift.RecurringGiftStatusId == MpRecurringGiftStatus.Paused
                    ? pushpayRecurringGift.UpdatedOn
                    : (DateTime?)null;

            mpRecurringGift.Notes = _pushpayService.GetRecurringGiftNotes(pushpayRecurringGift);

            if (pushpayRecurringGift.Links.ViewRecurringPayment != null && string.IsNullOrEmpty(mpRecurringGift.VendorDetailUrl))
            {
                mpRecurringGift.VendorDetailUrl = pushpayRecurringGift.Links.ViewRecurringPayment.Href;
            }

            if (pushpayRecurringGift.Links.MerchantViewRecurringPayment != null && string.IsNullOrEmpty(mpRecurringGift.VendorAdminDetailUrl))
            {
                mpRecurringGift.VendorAdminDetailUrl = pushpayRecurringGift.Links.MerchantViewRecurringPayment.Href;
            }

            return mpRecurringGift;
        }
    }
}
