using AutoMapper;
using ECER.Infrastructure.Common;

namespace ECER.Clients.RegistryPortal.Server.Communications
{
  public class CommunicationMapper : SecureProfile
  {
    public CommunicationMapper()
    {
      CreateMap<Managers.Registry.Contract.Communications.Communication, Communication>().ReverseMap();
      CreateMap<Managers.Registry.Contract.Communications.CommunicationDocument, CommunicationDocument>().ReverseMap();
    }
  }
}
