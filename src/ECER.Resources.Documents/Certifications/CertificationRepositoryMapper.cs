using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.Certifications;

internal class CertificationRepositoryMapper : Profile
{
  public CertificationRepositoryMapper()
  {
    CreateMap<ecer_Certificate, Certification>(MemberList.Destination)
     .ForMember(d => d.Id, opts => opts.MapFrom(s => s.ecer_CertificateId))
     .ForMember(d => d.Number, opts => opts.MapFrom(s => s.ecer_CertificateNumber))
     .ForMember(d => d.ExpiryDate, opts => opts.MapFrom(s => s.ecer_ExpiryDate))
     .ForMember(d => d.EffectiveDate, opts => opts.MapFrom(s => s.ecer_EffectiveDate))
     .ForMember(d => d.Date, opts => opts.MapFrom(s => s.ecer_Date))
     .ForMember(d => d.HasConditions, opts => opts.MapFrom(s => s.ecer_HasConditions))
     .ForMember(d => d.Level, opts => opts.MapFrom(s => s.ecer_CertificateLevel))
     .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.StatusCode))
     .ForMember(d => d.IneligibleReference, opts => opts.MapFrom(s => s.ecer_IneligibleReference));

    CreateMap<CertificateStatusCode, ecer_Certificate_StatusCode>()
    .ConvertUsingEnumMapping(opts => opts.MapByName(true))
    .ReverseMap();

    CreateMap<YesNoNull, ecer_YesNoNull>()
    .ConvertUsingEnumMapping(opts => opts.MapByName(true))
    .ReverseMap();
  }
}
