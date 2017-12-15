using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Crossroads.Service.Finance.Models;
using Crossroads.Service.Finance.Interfaces;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Models;
using RestSharp;
using RestSharp.Serializers;

namespace Crossroads.Service.Finance.Services
{
    public class DepositService : IDepositService
    {
        // TODO: Replace with int micro service url
        private Uri apiUri = new Uri(Environment.GetEnvironmentVariable("FINANCE_MS_ENDPOINT") ?? "https://gatewayint.crossroads.net/finance/api/paymentevent/settlement");
        //private Uri apiUri = new Uri("http://localhost:62545/api/");
        private readonly IDepositRepository _depositRepository;
        private readonly IMapper _mapper;
        private readonly IPushpayService _pushpayService;
        private readonly IRestClient _restClient;

        public DepositService(IDepositRepository depositRepository, IMapper mapper, IPushpayService pushpayService, IRestClient restClient = null)
        {
            _depositRepository = depositRepository;
            _mapper = mapper;
            _pushpayService = pushpayService;
            _restClient = restClient ?? new RestClient();
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
            // need to:
            // 1. Verify that these are inclusive of the specified day, not less than or greater than
            // 2. Determine the date range that makes sense here
            var startDate = DateTime.Now.AddDays(-7);
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

        public void SubmitDeposits(List<SettlementEventDto> deposits)
        {
            _restClient.BaseUrl = apiUri;
            var deposit = deposits.First();

            var request = new RestRequest(Method.POST)
            {
                Resource = $"paymentevent/settlement"
            };

            request.AddHeader("Content-Type", "application/json");
            request.JsonSerializer = new JsonSerializer(); // needed?
            request.RequestFormat = DataFormat.Json;
            request.AddBody(deposit);

            var response = _restClient.Execute(request);

            //foreach (var deposit in deposits)
            //{
            //    //var request = new RestRequest(Method.POST)
            //    //{
            //    //    Resource = $"paymentevent/settlement"
            //    //};

            //    //request.AddBody(deposit);

            //    //var response = _restClient.Execute(request);
            //}
        }

        // TODO: Consider merging this with the single call to get a deposit
        public List<DepositDto> GetDepositsByTransferIds(List<string> transferIds)
        {
            var mpDepositDtos = _depositRepository.GetDepositsByTransferIds(transferIds);
            var depositDtos = _mapper.Map<List<DepositDto>>(mpDepositDtos);
            return depositDtos;
        }
    }
}
