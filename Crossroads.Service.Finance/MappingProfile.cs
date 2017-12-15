using AutoMapper;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;
using Pushpay.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MpDeposit, DepositDto>();
        CreateMap<DepositDto, MpDeposit>();
        CreateMap<MpDonationBatch, DonationBatchDto>();
        CreateMap<DonationBatchDto, MpDonationBatch>();
        CreateMap<MpDonation, DonationDto>();
        CreateMap<DonationDto, MpDonation>();
        CreateMap<PushpayAmountDto, AmountDto>();
        CreateMap<PushpayLinkDto, LinkDto>();
        CreateMap<PushpayPaymentProcessorChargeDto, PaymentProcessorChargeDto>();
        CreateMap<PushpayPaymentsDto, PaymentsDto>();
        CreateMap<PushpaySettlementDto, SettlementEventDto>();
    }
}
