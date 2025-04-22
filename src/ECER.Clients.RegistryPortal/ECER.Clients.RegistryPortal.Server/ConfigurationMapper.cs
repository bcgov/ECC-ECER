using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server;

public class ConfigurationMapper : Profile
{
  public ConfigurationMapper()
  {
    CreateMap<Managers.Admin.Contract.Metadatas.Province, Province>().ReverseMap();
    CreateMap<Managers.Admin.Contract.Metadatas.Country, Country>().ReverseMap();
    CreateMap<Managers.Admin.Contract.Metadatas.SystemMessage, SystemMessage>();
    CreateMap<Managers.Admin.Contract.Metadatas.IdentificationType, IdentificationType>();
    CreateMap<Managers.Admin.Contract.Metadatas.PostSecondaryInstitution, PostSecondaryInstitution>().ReverseMap();
    CreateMap<Managers.Admin.Contract.Metadatas.OutOfProvinceCertificationType, OutOfProvinceCertificationType>().ReverseMap();
    CreateMap<Managers.Admin.Contract.Metadatas.CertificationComparison, CertificationComparison>().ReverseMap();
  }
}
