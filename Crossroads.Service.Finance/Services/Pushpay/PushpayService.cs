using System;
using System.Collections.Generic;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Models;
using Pushpay.Client;
using Pushpay.Models;

namespace Crossroads.Service.Finance.Services
{
    public class PushpayService : IPushpayService
    {
        private readonly IPushpayClient _pushpayClient;
        private readonly IMapper _mapper;

        public PushpayService(IPushpayClient pushpayClient, IMapper mapper)
        {
            _pushpayClient = pushpayClient;
            _mapper = mapper;
        }

        public PaymentsDto GetChargesForTransfer(string settlementKey)
        {
            var result = _pushpayClient.GetPushpayDonations(settlementKey);
            return _mapper.Map<PaymentsDto>(result);
        }

        public List<DepositDto> GetDepositsByDateRange(DateTime startDate, DateTime endDate)
        {
            var result = _pushpayClient.GetDepositByDateRange(startDate, endDate);
            return _mapper.Map<List<DepositDto>>(result);
        }
    }
}
