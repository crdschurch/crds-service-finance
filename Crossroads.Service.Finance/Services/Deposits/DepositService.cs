using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using Utilities.Logging;

namespace Crossroads.Service.Finance.Services
{
    public class DepositService : IDepositService
    {
        private readonly IDepositRepository _depositRepository;
        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IMapper _mapper;
        private readonly IPushpayService _pushpayService;
        private readonly IConfigurationWrapper _configurationWrapper;
        private readonly IDataLoggingService _dataLoggingService;

        private readonly int _depositProcessingOffset;
        private readonly string _pushpayWebEndpoint;

        public DepositService(IDepositRepository depositRepository,
                              IMapper mapper,
                              IPushpayService pushpayService,
                              IConfigurationWrapper configurationWrapper,
                              IDataLoggingService dataLoggingService)
        {
            _depositRepository = depositRepository;
            _mapper = mapper;
            _pushpayService = pushpayService;
            _configurationWrapper = configurationWrapper;
            _dataLoggingService = dataLoggingService;

            _depositProcessingOffset = _configurationWrapper.GetMpConfigIntValue("CRDS-FINANCE", "DepositProcessingOffset", true).GetValueOrDefault();
            _pushpayWebEndpoint = Environment.GetEnvironmentVariable("PUSHPAY_WEB_ENDPOINT");
        }

        public async Task<DepositDto> BuildDeposit(SettlementEventDto settlementEventDto)
        {
            var depositName = settlementEventDto.Name;
            var depositKey = settlementEventDto.Key;
            var existingDeposits = await _depositRepository.GetByName(depositName);

            // append a number to the deposit, based on how many deposits already exist by that name
            // with the datetime and deposit type
            if (existingDeposits.Count < 10)
            {
                depositName = depositName + "00" + existingDeposits.Count;
            }
            else if (existingDeposits.Count >= 10 && existingDeposits.Count < 100)
            {
                depositName = depositName + "0" + existingDeposits.Count;
            }
            else if (existingDeposits.Count >= 100 && existingDeposits.Count < 999)
            {
                depositName = depositName + existingDeposits.Count;
            }
            else if (existingDeposits.Count >= 1000)
            {
                throw new Exception("Over 999 deposits for same time period");
            }

            // limit deposit name to 15 chars or less to comply with GP export
            if (depositName.Length > 14)
            {
                var truncateValue = depositName.Length - 14;
                depositName = depositName.Remove(0, truncateValue);
            }

            var estDepositDate = settlementEventDto.EstimatedDepositDate.ToString("MM-dd-yyyy");
            var depositDto = new DepositDto
            {
                // Account number must be non-null, and non-empty; using a single space to fulfill this requirement
                AccountNumber = " ",
                BatchCount = 1,
                DepositDateTime = DateTime.Now,
                DepositName = depositName,
                DepositTotalAmount = Decimal.Parse(settlementEventDto.TotalAmount.Amount),
                DepositAmount = Decimal.Parse(settlementEventDto.TotalAmount.Amount),
                Exported = false,
                Notes = null,
                ProcessorTransferId = depositKey,
                VendorDetailUrl = $"{_pushpayWebEndpoint}/pushpay/0/settlements?includeCardSettlements=True&includeAchSettlements=True&fromDate={estDepositDate}&toDate={estDepositDate}",
            };
            return depositDto;
        }

        public async Task<DepositDto> SaveDeposit(DepositDto depositDto)
        {
            var mpDepositResult = await _depositRepository.CreateDeposit(_mapper.Map<MpDeposit>(depositDto));
            var mappedObject = _mapper.Map<DepositDto>(mpDepositResult);
            return mappedObject;
        }

        public async Task<DepositDto> GetDepositByProcessorTransferId(string key)
        {
            return _mapper.Map<DepositDto>(await _depositRepository.GetDepositByProcessorTransferId(key));
        }

        // this will pull desposits by a date range and determine which ones we need to create in the system
        public async Task<List<SettlementEventDto>> SyncDeposits()
        {
            // we look back however many days are specified in the mp config setting
            var startDate = DateTime.Now.AddDays(-(_depositProcessingOffset));
            var endDate = DateTime.Now;

            Console.WriteLine($"Checking pushpay for deposits between {startDate} and {endDate}");

            var logEventEntry = new LogEventEntry(LogEventType.depositSearchDateRange);
            logEventEntry.Push("startDate", startDate);
            logEventEntry.Push("endDate", endDate);
            _dataLoggingService.LogDataEvent(logEventEntry);

            var depositDtos = await GetDepositsForSync(startDate, endDate);

            if (depositDtos == null || !depositDtos.Any())
            {
                return null;
            }

            return depositDtos;
        }

        public async Task<List<SettlementEventDto>> GetDepositsForSync(DateTime startDate, DateTime endDate)
        {
            var deposits = await _pushpayService.GetDepositsByDateRange(startDate, endDate);
            Console.WriteLine($"{deposits.Count} found in pushpay");

            var depositsFoundEntry = new LogEventEntry(LogEventType.incomingPushpayWebhook);
            depositsFoundEntry.Push("depositsFoundCount", deposits.Count);
            _dataLoggingService.LogDataEvent(depositsFoundEntry);

            if (!deposits.Any())
            {
                return null;
            }

            var transferIds = deposits.Select(r => "'" + r.Key + "'").ToList();

            // check to see if any of the deposits we're pulling over have already been deposited
            var existingDeposits = await _depositRepository.GetByTransferIds(transferIds);
            Console.WriteLine($"{existingDeposits.Count} of these deposits found in MP");

            var alreadyDepositedEntry = new LogEventEntry(LogEventType.depositsAlreadyDeposited);
            alreadyDepositedEntry.Push("countDepositsAlreadyDeposited", existingDeposits.Count);
            _dataLoggingService.LogDataEvent(alreadyDepositedEntry);

            var existingDepositIds = existingDeposits.Select(r => r.ProcessorTransferId).ToList();

            var depositsToProcess = new List<SettlementEventDto>();

            foreach (var deposit in deposits)
            {
                if (!existingDepositIds.Contains(deposit.Key))
                {
                    Console.WriteLine($"Deposit {deposit.Key} ProcessorTransferId not found in MP, adding to sync list");

                    var newDepositEntry = new LogEventEntry(LogEventType.newDepositToSync);
                    newDepositEntry.Push("newDeposit", deposit.Key);
                    _dataLoggingService.LogDataEvent(newDepositEntry);

                    depositsToProcess.Add(deposit);
                }
                else
                {
                    Console.WriteLine($"Deposit {deposit.Key} found in MP, skipping");

                    var previouslySyncedDepositEntry = new LogEventEntry(LogEventType.previouslySyncedDeposit);
                    previouslySyncedDepositEntry.Push("oldDeposit", deposit.Key);
                    _dataLoggingService.LogDataEvent(previouslySyncedDepositEntry);
                }
            }

            return depositsToProcess;
        }
    }
}
