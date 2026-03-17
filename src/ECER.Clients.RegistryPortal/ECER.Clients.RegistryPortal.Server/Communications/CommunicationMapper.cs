using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Utilities.ObjectStorage.Providers;

namespace ECER.Clients.RegistryPortal.Server.Communications
{
  public class CommunicationMapper : SecureProfile
  {
    public CommunicationMapper()
    {
      CreateMap<Managers.Registry.Contract.Communications.Communication, Communication>().ReverseMap();
      CreateMap<Managers.Registry.Contract.Communications.CommunicationDocument, CommunicationDocument>()
      .ReverseMap()
      .ForMember(s => s.EcerWebApplicationType, opts => opts.MapFrom(_ => EcerWebApplicationType.Registry)); //always set the web application type to Registry when it's coming from registry;
    }
  }
}
