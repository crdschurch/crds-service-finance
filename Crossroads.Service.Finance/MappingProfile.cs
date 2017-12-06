using AutoMapper;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;
using Pushpay.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MpContact, ContactDto>();
        CreateMap<MpDeposit, DepositDto>();
        CreateMap<PushpayAmountDto, AmountDto>();
        CreateMap<PushpayLinkDto, LinkDto>();
        CreateMap<PushpayPaymentProcessorChargeDto, PaymentProcessorChargeDto>();
        CreateMap<PushpayPaymentsDto, PaymentsDto>();
    }
}
