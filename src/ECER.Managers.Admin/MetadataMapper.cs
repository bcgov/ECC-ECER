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
    CreateMap<DefaultContent, Contract.Metadatas.DefaultContent>().ReverseMap();
    CreateMap<IEnumerable<DynamicsConfig>, Contract.Metadatas.DynamicsConfig>()
      .ForCtorParam(nameof(Contract.Metadatas.DynamicsConfig.ICRAFeatureEnabled), opt => opt.MapFrom(src => FeatureEnabled(src, "ICRA Feature")
      ));
  }

  private static bool FeatureEnabled(IEnumerable<DynamicsConfig> src, string key)
  {
    var feature = src.FirstOrDefault(item => item.Key == key);
    return feature != null && feature.Value?.ToUpper() == "ON";
  }
}
