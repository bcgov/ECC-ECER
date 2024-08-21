using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.Communications
{
  public class CommunicationMapper : Profile
  {
    public CommunicationMapper()
    {
      CreateMap<Managers.Registry.Contract.Communications.Communication, Communication>().ReverseMap();
      CreateMap<Managers.Registry.Contract.Communications.CommunicationDocument, CommunicationDocument>().ReverseMap();
    }
  }
}
