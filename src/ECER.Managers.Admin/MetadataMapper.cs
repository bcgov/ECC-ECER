using AutoMapper;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Managers.Admin;

internal class MetadataMapper : Profile
{
  public MetadataMapper()
  {
    CreateMap<Country, Contract.Metadatas.Country>().ReverseMap();
    CreateMap<Province, Contract.Metadatas.Province>().ReverseMap();
    CreateMap<PostSecondaryInstitution, Contract.Metadatas.PostSecondaryInstitution>().ReverseMap();
    CreateMap<SystemMessage, Contract.Metadatas.SystemMessage>();
    CreateMap<IdentificationType, Contract.Metadatas.IdentificationType>();
    CreateMap<PostSecondaryInstitutionsQuery, Contract.Metadatas.PostSecondaryInstitutionsQuery>().ReverseMap();
    CreateMap<CertificationComparison, Contract.Metadatas.CertificationComparison>().ReverseMap();
    CreateMap<OutOfProvinceCertificationType, Contract.Metadatas.OutOfProvinceCertificationType>().ReverseMap();
  }
}
