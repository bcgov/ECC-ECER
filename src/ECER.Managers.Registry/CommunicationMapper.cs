using AutoMapper;
using ECER.Resources.Accounts.Communications; 

namespace ECER.Managers.Registry
{
  internal class CommunicationMapper : Profile
  {

    public CommunicationMapper()
    {
      CreateMap<Communication, ECER.Managers.Registry.Contract.Communications.Communication>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? string.Empty))
           .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Body))
           .ReverseMap()
           .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Text));

      CreateMap<CommunicationsStatus, ECER.Managers.Registry.Contract.Communications.CommunicationsStatus>()
        .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
        .ForMember(dest => dest.HasUnread, opt => opt.MapFrom(src => src.HasUnread))
        .ReverseMap();
    }
  }
}
