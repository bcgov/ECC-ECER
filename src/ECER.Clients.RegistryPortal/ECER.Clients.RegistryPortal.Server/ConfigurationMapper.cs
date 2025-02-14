using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server;

public class ConfigurationMapper : Profile
{
  public ConfigurationMapper()
  {
    CreateMap<Managers.Admin.Contract.Metadatas.Province, Province>();
    CreateMap<Managers.Admin.Contract.Metadatas.Country, Country>();
    CreateMap<Managers.Admin.Contract.Metadatas.SystemMessage, SystemMessage>();
    CreateMap<Managers.Admin.Contract.Metadatas.IdentificationType, IdentificationType>();
  }
}
