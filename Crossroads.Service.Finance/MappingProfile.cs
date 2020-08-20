using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AutoMapper;
using Crossroads.Service.Finance.Constants;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;
using Pushpay.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MpPledge, PledgeDto>();
        CreateMap<MpContact, ContactDto>();
        CreateMap<MpContactRelationship, ContactRelationship>();
        CreateMap<MpDeposit, DepositDto>();
        CreateMap<DepositDto, MpDeposit>();
        CreateMap<MpDonationBatch, DonationBatchDto>();
        CreateMap<DonationBatchDto, MpDonationBatch>();
        CreateMap<MpDonation, DonationDto>();
        CreateMap<DonationDto, MpDonation>();
        CreateMap<PushpayAmountDto, AmountDto>();
        CreateMap<PushpayLinkDto, LinkDto>();
        CreateMap<PushpayLinksDto, LinksDto>();
        CreateMap<PushpayPaymentDto, PaymentDto>();
        CreateMap<PushpayRefundPaymentDto, RefundPaymentDto>();
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
                r.Links != null && r.Links.ViewRecurringPayment != null ? r.Links.ViewRecurringPayment.Href : null
            ))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.UpdatedOn));
        CreateMap<MpRecurringGift, RecurringGiftDto>();
        CreateMap<MpDonationDetail, DonationDetailDto>()
            .ForMember(dest => dest.AccountNumber, opt => opt.ResolveUsing(r =>
            {
                try
                {
                    if (!String.IsNullOrEmpty(r.AccountNumber) && r.AccountNumber.Length >= 4)
                    {
                        // get last four characters, which is max of what we want to show
                        var formatted = r.AccountNumber.Substring(r.AccountNumber.Length - 4, 4);
                        // pushpay bank accounts have bullets in two of the last four, so remove those
                        return Regex.Replace(formatted, "[^0-9]", "");
                    }
                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        ));

        CreateMap<DonationDetailDto, MpDonationDetail>();
        CreateMap<MpContactAddress, ContactAddressDto>();
        CreateMap<PushpayTransactionBaseDto, MpDonorAccount>()
            .ForMember(dest => dest.AccountNumber,
                opt => opt.ResolveUsing(p =>
                    p.PaymentMethodType.ToLower() == "ach" ? p.Account.Reference : p.Card.Reference))
            .ForMember(dest => dest.InstitutionName,
                opt => opt.ResolveUsing(p =>
                {
                    if (p.PaymentMethodType.ToLower() == "ach")
                    {
                        return p.Account.BankName;
                    }

                    switch (p.Card.Brand)
                    {
                        case "VISA":
                            return "Visa";
                        case "Discover":
                            return "Discover";
                        case "Amex":
                            return "AmericanExpress";
                        case "MasterCard":
                            return "MasterCard";
                        default:
                            return "";
                    }
                }))
            .ForMember(dest => dest.RoutingNumber,
                opt => opt.ResolveUsing(p =>
                    p.PaymentMethodType.ToLower() == "ach" ? p.Account.RoutingNumber : null)
            )
            .ForMember(dest => dest.NonAssignable, opt => opt.ResolveUsing(p => false))
            .ForMember(dest => dest.DomainId, opt => opt.ResolveUsing(p => 1))
            .ForMember(dest => dest.Closed, opt => opt.ResolveUsing(p => false))
            .ForMember(dest => dest.ProcessorId, opt => opt.ResolveUsing(p => p.Payer.Key))
            .ForMember(dest => dest.ProcessorTypeId,
                opt => opt.ResolveUsing(p => ProcessorTypes.PushpayProcessorTypeId))
            .ForMember(dest => dest.AccountTypeId, opts => opts.ResolveUsing(p =>
            {
                switch (p.PaymentMethodType.ToLower())
                {
                    case "ach":
                        switch (p.Account.AccountType)
                        {
                            case "Checking":
                                return MpAccountTypes.Checkings;
                            case "Savings":
                                return MpAccountTypes.Savings;
                        }

                        break;
                    case "creditcard":
                        return MpAccountTypes.CreditCard;
                }
                return 0;
            }));

    }
}
