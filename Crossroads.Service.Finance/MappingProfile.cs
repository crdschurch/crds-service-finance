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
        CreateMap<PushpayPaymentDto, PaymentDto>();
        CreateMap<PushpayPaymentsDto, PaymentsDto>();
        CreateMap<PushpaySettlementDto, SettlementEventDto>();
        CreateMap<PushpaySettlementDto, SettlementDto>();
        CreateMap<PushpayRecurringGiftDto, MpRecurringGift>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
            .ForMember(d => d.FrequencyId, o => o.ResolveUsing(r => {
                switch (r.Schedule.Frequency)
                {
                    case "Weekly":
                        return 1;
                    case "Monthly":
                        return 2;
                    case "FirstAndFifteenth":
                        return 3;
                    case "Fortnightly":
                        return 4;
                    default:
                        return 0;
                }
            }));
            // TODO additional mappings
        CreateMap<MpRecurringGift, RecurringGiftDto>();
    }
}
