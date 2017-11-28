using AutoMapper;
using Crossroads.Service.Template.Models;
using MinistryPlatform.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MpContact, ContactDto>();
    }
}
