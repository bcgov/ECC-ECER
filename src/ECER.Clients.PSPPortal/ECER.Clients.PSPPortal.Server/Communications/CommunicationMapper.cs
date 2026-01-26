using AutoMapper;
using AutoMapper.Extensions.EnumMapping;

namespace ECER.Clients.PSPPortal.Server.Communications
{
  public class CommunicationMapper : Profile
  {
    public CommunicationMapper()
    {
      CreateMap<Managers.Registry.Contract.Communications.Communication, Communication>().ReverseMap();
      CreateMap<Managers.Registry.Contract.Communications.CommunicationDocument, CommunicationDocument>().ReverseMap();
      CreateMap<Managers.Registry.Contract.Communications.CommunicationCategory, CommunicationCategory>()
        .ConvertUsingEnumMapping(opts => opts.MapByName(true))
        .ReverseMap();
    }
  }
}

