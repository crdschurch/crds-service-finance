using System;
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
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Schedule.StartDate))
            .ForMember(dest => dest.SubscriptionId, opt => opt.MapFrom(src => src.PaymentToken))
            .ForMember(dest => dest.FrequencyId, opt => opt.ResolveUsing(r =>
            {
                switch (r.Schedule.Frequency)
                {
                    case "Weekly":
                        return MpRecurringFrequency.Weekly;
                    case "Monthly":
                        return MpRecurringFrequency.Monthly;
                    case "FirstAndFifteenth":
                        return MpRecurringFrequency.FirstAndFifteenth;
                    case "Fortnightly":
                        return MpRecurringFrequency.EveryOtherWeek;
                    default:
                        return (int?) null;
                }
            }))
            .ForMember(dest => dest.DayOfMonth, opt => opt.ResolveUsing(r =>
                {
                    switch (r.Schedule.Frequency)
                    {
                        case "Monthly":
                            return r.Schedule.StartDate.Day;
                        default:
                            return (int?) null;
                    }
                }
            ))
            .ForMember(dest => dest.DayOfWeek, opt => opt.ResolveUsing(r =>
                {
                    switch (r.Schedule.Frequency)
                    {
                        case "Weekly":
                        case "Fortnightly":
                            return MpRecurringGiftDays.GetMpRecurringGiftDay(r.Schedule.StartDate);
                        default:
                            return (int?) null;
                    }
                }
            ))
            .ForMember(dest => dest.VendorDetailUrl, opt => opt.ResolveUsing(r =>
                r.Links.ViewRecurringPayment.Href
            ));
        CreateMap<MpRecurringGift, RecurringGiftDto>();
    }
}
