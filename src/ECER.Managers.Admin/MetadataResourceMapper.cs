using AutoMapper;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Managers.Admin;

internal class MetadataResourceMapper : Profile
{
  public MetadataResourceMapper()
  {
    CreateMap<Province, Contract.MetadataResources.Province>();
  }
}
