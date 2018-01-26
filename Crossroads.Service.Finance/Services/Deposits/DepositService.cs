using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using RestSharp;
using RestSharp.Serializers;

namespace Crossroads.Service.Finance.Services
{
    public class DepositService : IDepositService
    {
        private readonly IDepositRepository _depositRepository;
        private readonly IMapper _mapper;
        private readonly IPushpayService _pushpayService;
        private readonly IRestClient _restClient;
        private readonly int _depositProcessingOffset;
        private readonly string _financePath;
        private readonly string _pushpayDepositEndpoint;

        public DepositService(IDepositRepository depositRepository, IMapper mapper, IPushpayService pushpayService, IConfigurationWrapper configurationWrapper, IRestClient restClient = null)
        {
            _depositRepository = depositRepository;
            _mapper = mapper;
            _pushpayService = pushpayService;
            _restClient = restClient ?? new RestClient();

            _financePath = Environment.GetEnvironmentVariable("FINANCE_PATH") ??
                               configurationWrapper.GetMpConfigValue("CRDS-FINANCE", "FinanceMicroservicePath", true);

            _depositProcessingOffset = configurationWrapper.GetMpConfigIntValue("CRDS-FINANCE", "DepositProcessingOffset", true).GetValueOrDefault();
            _pushpayDepositEndpoint = Environment.GetEnvironmentVariable("PUSHPAY_DEPOSIT_ENDPOINT");
        }

        public DepositDto CreateDeposit(SettlementEventDto settlementEventDto, string depositName)
        {
            var existingDeposits = _depositRepository.GetDepositNamesByDepositName(depositName);

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

            var estDepositDate = String.Format("{MM-dd-yyyy}", settlementEventDto.EstimatedDepositDate);
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
                ProcessorTransferId = $"{_pushpayDepositEndpoint}?includeCardSettlements=True&includeAchSettlements=True&fromDate={estDepositDate}&toDate={estDepositDate}",
            };

            return depositDto;
        }

        public DepositDto SaveDeposit(DepositDto depositDto)
        {
            var mpDepositResult = _depositRepository.CreateDeposit(_mapper.Map<MpDeposit>(depositDto));
            var mappedObject = _mapper.Map<DepositDto>(mpDepositResult);
            return mappedObject;
        }

        public DepositDto GetDepositByProcessorTransferId(string key)
        {
            return _mapper.Map<DepositDto>(_depositRepository.GetDepositByProcessorTransferId(key));
        }

        // this will pull desposits by a date range and determine which ones we need to create in the system
        public void SyncDeposits(string hostName)
        {
            // we look back however many days are specified in the mp config setting
            var startDate = DateTime.Now.AddDays(-(_depositProcessingOffset));
            var endDate = DateTime.Now;

            var depositDtos = GetDepositsForSync(startDate, endDate);

            if (depositDtos == null || !depositDtos.Any())
            {
                return;
            }

            SubmitDeposits(depositDtos, hostName);
        }

        public List<SettlementEventDto> GetDepositsForSync(DateTime startDate, DateTime endDate)
        {
            var deposits = _pushpayService.GetDepositsByDateRange(startDate, endDate);

            if (!deposits.Any())
            {
                return null;
            }

            var transferIds = deposits.Select(r => "'" + r.Key + "'").ToList();

            // check to see if any of the deposits we're pulling over have already been deposited
            var existingDeposits = _depositRepository.GetDepositsByTransferIds(transferIds);
            var existingDepositIds = existingDeposits.Select(r => r.ProcessorTransferId).ToList();

            var depositsToProcess = new List<SettlementEventDto>();

            foreach (var deposit in deposits)
            {
                if (!existingDepositIds.Contains(deposit.Key))
                {
                    depositsToProcess.Add(deposit);
                }
            }

            return depositsToProcess;
        }

        // returns the list of all deposits that woould be pulled, if we weren't checking what is in the system -
        // mostly to support testing and auditing
        public List<SettlementEventDto> GetDepositsForSyncRaw(DateTime startDate, DateTime endDate)
        {
            var deposits = _pushpayService.GetDepositsByDateRange(startDate, endDate);
            return deposits;
        }

        public List<SettlementEventDto> GetDepositsForPendingSync()
        {
            // we look back however many days are specified in the mp config setting
            var startDate = DateTime.Now.AddDays(-(_depositProcessingOffset));
            var endDate = DateTime.Now;

            var deposits = _pushpayService.GetDepositsByDateRange(startDate, endDate);
            return deposits;
        }

        public void SubmitDeposits(List<SettlementEventDto> deposits, string hostName)
        {
            // TODO: There is some code smell around this - determine if there is a better way to handle this
            _restClient.BaseUrl = hostName.Contains("localhost") ? new Uri("http://" + hostName) : new Uri("https://" + hostName);

            foreach (var deposit in deposits)
            {
                var request = new RestRequest(Method.POST)
                {
                    Resource = _financePath + "paymentevent/settlement"
                };

                request.AddHeader("Content-Type", "application/json");
                request.JsonSerializer = new JsonSerializer(); // needed?
                request.RequestFormat = DataFormat.Json;
                request.AddBody(deposit);

                var response = _restClient.Execute(request);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"Could not find server for {_restClient.BaseUrl} & {request.Resource}");
                }
            }
        }
    }
}
