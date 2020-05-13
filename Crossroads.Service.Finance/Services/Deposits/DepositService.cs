using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProcessLogging.Models;
using ProcessLogging.Transfer;

namespace Crossroads.Service.Finance.Services
{
    public class DepositService : IDepositService
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IDepositRepository _depositRepository;
        private readonly IRecurringGiftRepository _recurringGiftRepository;
        private readonly IMapper _mapper;
        private readonly IPushpayService _pushpayService;
        private readonly IConfigurationWrapper _configurationWrapper;
        private readonly IProcessLogger _processLogger;

        private readonly int _depositProcessingOffset;
        private readonly string _pushpayWebEndpoint;

        public DepositService(IDepositRepository depositRepository,
                              IMapper mapper,
                              IPushpayService pushpayService,
                              IConfigurationWrapper configurationWrapper,
                              IProcessLogger processLogger)
        {
            _depositRepository = depositRepository;
            _mapper = mapper;
            _pushpayService = pushpayService;
            _configurationWrapper = configurationWrapper;
            _processLogger = processLogger;

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
                DepositDateTime = DateTime.Parse(estDepositDate),
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

            var depositDtos = await GetDepositsForSync(startDate, endDate);
            var depositCount = (depositDtos == null || !depositDtos.Any()) ? 0 : depositDtos.Count;

            //Console.WriteLine($"Checking pushpay for deposits between {startDate} and {endDate}. Found {depositCount}");
            //_logger.Info($"Checking pushpay for deposits between {startDate} and {endDate}. Found {depositCount}");

            var checkingPushpayDepositsMessage = new ProcessLogMessage(ProcessLogConstants.MessageType.pushpayDepositCount)
            {
                MessageData = $"Checking pushpay for deposits between {startDate} and {endDate}. Found {depositCount}."
            };
            _processLogger.SaveProcessLogMessage(checkingPushpayDepositsMessage);

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

            if (!deposits.Any())
            {
                return null;
            }

            var transferIds = deposits.Select(r => "'" + r.Key + "'").ToList();

            // check to see if any of the deposits we're pulling over have already been deposited
            var existingDeposits = await _depositRepository.GetByTransferIds(transferIds);

            var existingDepositIds = existingDeposits.Select(r => r.ProcessorTransferId).ToList();
            var existingDepositList = String.Join(", ", existingDepositIds);

            //Console.WriteLine($"These deposits already exist in MP and will be skipped: {existingDepositList}");
            //_logger.Info($"These deposits already exist in MP and will be skipped: {existingDepositList}");

            var skippedDepositsMessage = new ProcessLogMessage(ProcessLogConstants.MessageType.depositsToSkip)
            {
                MessageData = $"These deposits already exist in MP and will be skipped: {existingDepositList}"
            };
            _processLogger.SaveProcessLogMessage(skippedDepositsMessage);

            var depositsToProcess = new List<SettlementEventDto>();

            foreach (var deposit in deposits)
            {
                if (!existingDepositIds.Contains(deposit.Key))
                {
                    depositsToProcess.Add(deposit);
                }
            }

            var depositsToProcessList = String.Join(", ", depositsToProcess.Select(r => r.Name));
            //Console.WriteLine($"These deposits do not exist in MP and will be created: {depositsToProcessList}");
            //_logger.Info($"These deposits do not exist in MP and will be created: {depositsToProcessList}");

            var depositsToCreateMessage = new ProcessLogMessage(ProcessLogConstants.MessageType.depositsToCreate)
            {
                MessageData = $"These deposits do not exist in MP and will be created: {depositsToProcessList}"
            };
            _processLogger.SaveProcessLogMessage(depositsToCreateMessage);

            return depositsToProcess;
        }
    }
}
