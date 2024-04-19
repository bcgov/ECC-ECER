using AutoMapper;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Managers.Registry;

internal class MetadataResourceMapper : Profile
{
  public MetadataResourceMapper()
  {
    CreateMap<Province, Contract.MetadataResources.Province>();
  }
}
