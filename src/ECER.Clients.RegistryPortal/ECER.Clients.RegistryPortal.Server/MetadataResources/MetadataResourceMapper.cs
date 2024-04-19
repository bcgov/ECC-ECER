using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.MetadataResources;

public class MetadataResourceMapper : Profile
{
  public MetadataResourceMapper()
  {
    CreateMap<Managers.Registry.Contract.MetadataResources.Province, Province>();
  }
}
