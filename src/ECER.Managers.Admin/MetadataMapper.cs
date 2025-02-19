using AutoMapper;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Managers.Admin;

internal class MetadataMapper : Profile
{
  public MetadataMapper()
  {
    CreateMap<Province, Contract.Metadatas.Province>();
    CreateMap<Country, Contract.Metadatas.Country>();
    CreateMap<SystemMessage, Contract.Metadatas.SystemMessage>();
    CreateMap<IdentificationType, Contract.Metadatas.IdentificationType>();
    CreateMap<PostSecondaryInstitution, Contract.Metadatas.PostSecondaryInstitution>();
    CreateMap<PostSecondaryInstitutionsQuery, Contract.Metadatas.PostSecondaryInstitutionsQuery>().ReverseMap();
  }
}
