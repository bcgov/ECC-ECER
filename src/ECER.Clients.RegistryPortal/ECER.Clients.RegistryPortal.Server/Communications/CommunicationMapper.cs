using AutoMapper;
using ECER.Utilities.ObjectStorage.Providers;

namespace ECER.Clients.RegistryPortal.Server.Communications
{
  public class CommunicationMapper : Profile
  {
    public CommunicationMapper()
    {
      CreateMap<Managers.Registry.Contract.Communications.Communication, Communication>().ReverseMap();
      CreateMap<Managers.Registry.Contract.Communications.CommunicationDocument, CommunicationDocument>()
        .ReverseMap()
        .ForMember(s => s.EcerWebApplicationType, opts => opts.MapFrom(_ => EcerWebApplicationType.Registry)); //always set the web application type to Registry when it's coming from registry
    }
  }
}
