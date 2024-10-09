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
      CreateMap<Managers.Registry.Contract.Certifications.CertificationFile, CertificationFile>().ReverseMap();
      CreateMap<Managers.Registry.Contract.Certifications.Certification, CertificationLookupResponse>()
        .ForCtorParam(nameof(CertificationLookupResponse.Id),
        opt => opt.MapFrom(s => s.Id))
      .ForMember(d => d.Name, opts => opts.MapFrom(s => s.Name))
      .ForMember(d => d.RegistrationNumber, opts => opts.MapFrom(s => s.Number))
      .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.StatusCode))
      .ForMember(d => d.LevelName, opts => opts.MapFrom(s => s.LevelName))
      .ForMember(d => d.ExpiryDate, opts => opts.MapFrom(s => s.ExpiryDate))
      .ForMember(d => d.HasConditions, opts => opts.MapFrom(s => s.HasConditions))
      .ForMember(d => d.CertificateConditions, opts => opts.MapFrom(s => s.CertificateConditions));

      CreateMap<Managers.Registry.Contract.Certifications.CertificateCondition, CertificateCondition>();
    }
  }
}
