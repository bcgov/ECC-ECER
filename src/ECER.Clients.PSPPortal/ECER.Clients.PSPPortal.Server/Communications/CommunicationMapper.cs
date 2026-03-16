using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Utilities.ObjectStorage.Providers;

namespace ECER.Clients.PSPPortal.Server.Communications
{
  public class CommunicationMapper : Profile
  {
    public CommunicationMapper()
    {
      CreateMap<Managers.Registry.Contract.Communications.Communication, Communication>().ReverseMap();
      CreateMap<Managers.Registry.Contract.Communications.CommunicationDocument, CommunicationDocument>()
        .ReverseMap()
        .ForMember(s => s.EcerWebApplicationType, opts => opts.MapFrom(d => EcerWebApplicationType.PSP)); //default to PSP when saving anything from the frontend
      CreateMap<Managers.Registry.Contract.Communications.CommunicationCategory, CommunicationCategory>()
        .ConvertUsingEnumMapping(opts => opts.MapByName(true))
        .ReverseMap();
    }
  }
}
