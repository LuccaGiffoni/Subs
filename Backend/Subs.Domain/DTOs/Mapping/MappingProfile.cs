using AutoMapper;
using Subs.Domain.Enums;
using Subs.Domain.Models;
using Subs.Domain.Models.History;
using Subs.Domain.Models.Messages;
using Subs.Domain.Models.SubscriptionComponents;
using Subs.Domain.Models.SubscriptionComponents.PaymentComponents;

namespace Subs.Domain.DTOs.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Subscription, SubscriptionDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<EStatus>(src.Status)));

        CreateMap<Client, ClientDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ReverseMap()
            .ForMember(dest => dest.Subscriptions, opt => opt.Ignore());

        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method.ToString()))
            .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Method, opt => opt.MapFrom(src => Enum.Parse<EPaymentMethod>(src.Method)))
            .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => Enum.Parse<EPaymentFrequency>(src.Frequency)));

        CreateMap<Discount, DiscountDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<EDiscountType>(src.Type)));

        CreateMap<Currency, CurrencyDto>().ReverseMap();

        CreateMap<SubscriptionEventHistory, SubscriptionEventHistoryDto>()
            .ForMember(dest => dest.Operation, opt => opt.MapFrom(src => src.Operation.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Operation, opt => opt.MapFrom(src => Enum.Parse<EOperation>(src.Operation)));

        CreateMap<ClientEventHistory, ClientEventHistoryDto>()
            .ForMember(dest => dest.Operation, opt => opt.MapFrom(src => src.Operation.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Operation, opt => opt.MapFrom(src => Enum.Parse<EOperation>(src.Operation)));

        CreateMap<SubscriptionMessage, SubscriptionMessageDto>()
            .ForMember(dest => dest.Operation, opt => opt.MapFrom(src => src.Operation.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Operation, opt => opt.MapFrom(src => Enum.Parse<EOperation>(src.Operation)));

        CreateMap<ClientMessage, ClientMessageDto>()
            .ForMember(dest => dest.Operation, opt => opt.MapFrom(src => src.Operation.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Operation, opt => opt.MapFrom(src => Enum.Parse<EOperation>(src.Operation)));
    }
}