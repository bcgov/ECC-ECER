using AutoMapper;

#pragma warning disable 8631

namespace ECER.Clients.RegistryPortal.Server.Certifications
{
  public class CertificationMapper : Profile
  {
    public CertificationMapper()
    {
      CreateMap<Managers.Registry.Contract.Certifications.Certification, Certification>().ReverseMap();
      CreateMap<Managers.Registry.Contract.Certifications.CertificationLevel, CertificationLevel>().ReverseMap();
      CreateMap<Managers.Registry.Contract.Certifications.CertificationFile, CertificationFile>()
          .ForMember(d => d.Url, opts => opts.MapFrom<UrlResolver>())
          .ReverseMap();
    }
  }
}
