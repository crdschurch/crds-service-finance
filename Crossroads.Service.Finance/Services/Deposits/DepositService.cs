using System;
using System.Collections.Generic;
using System.Linq;
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
        // TODO: Replace with int micro service url
        private Uri apiUri = new Uri(Environment.GetEnvironmentVariable("FINANCE_MS_ENDPOINT") ?? "https://gatewayint.crossroads.net/finance/api/");
        private readonly IDepositRepository _depositRepository;
        private readonly IMapper _mapper;
        private readonly IPushpayService _pushpayService;
        private readonly IRestClient _restClient;
        private readonly int _depositProcessingOffset;

        public DepositService(IDepositRepository depositRepository, IMapper mapper, IPushpayService pushpayService, IConfigurationWrapper configurationWrapper, IRestClient restClient = null)
        {
            _depositRepository = depositRepository;
            _mapper = mapper;
            _pushpayService = pushpayService;
            _restClient = restClient ?? new RestClient();

            _depositProcessingOffset = configurationWrapper.GetMpConfigIntValue("CRDS-FINANCE", "DepositProcessingOffset", true).GetValueOrDefault();
        }

        public DepositDto CreateDeposit(SettlementEventDto settlementEventDto, string depositName)
        {
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
                ProcessorTransferId = settlementEventDto.Key
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
        public void SyncDeposits()
        {
            // we look back however many days are specified in the mp config setting
            var startDate = DateTime.Now.AddDays(-(_depositProcessingOffset));
            var endDate = DateTime.Now;

            var depositDtos = GetDepositsForSync(startDate, endDate);
            SubmitDeposits(depositDtos);
        }

        public List<SettlementEventDto> GetDepositsForSync(DateTime startDate, DateTime endDate)
        {
            var deposits = _pushpayService.GetDepositsByDateRange(startDate, endDate);
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

        public void SubmitDeposits(List<SettlementEventDto> deposits)
        {
            _restClient.BaseUrl = apiUri;

            foreach (var deposit in deposits)
            {
                var request = new RestRequest(Method.POST)
                {
                    Resource = $"paymentevent/settlement"
                };

                request.AddHeader("Content-Type", "application/json");
                request.JsonSerializer = new JsonSerializer(); // needed?
                request.RequestFormat = DataFormat.Json;
                request.AddBody(deposit);

                var response = _restClient.Execute(request);
            }
        }
    }
}
