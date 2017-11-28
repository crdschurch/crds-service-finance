using AutoMapper;
using Crossroads.Service.Finance.Models;
using MinistryPlatform.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MpContact, ContactDto>();
    }
}
