using AutoMapper;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Managers.Admin;

internal class MetadataMapper : Profile
{
  public MetadataMapper()
  {
    CreateMap<Province, Contract.Metadatas.Province>();
    CreateMap<Country, Contract.Metadatas.Country>();
  }
}
