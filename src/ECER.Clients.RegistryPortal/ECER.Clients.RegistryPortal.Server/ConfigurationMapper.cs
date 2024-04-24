using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server;

public class ConfigurationMapper : Profile
{
  public ConfigurationMapper()
  {
    CreateMap<Managers.Admin.Contract.Metadatas.Province, Province>();
  }
}
