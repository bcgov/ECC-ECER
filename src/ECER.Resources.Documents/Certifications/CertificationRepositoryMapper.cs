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
     .ForMember(d => d.Name, opts => opts.MapFrom(s => s.ecer_RegistrantidName))
     .ForMember(d => d.Number, opts => opts.MapFrom(s => s.ecer_CertificateNumber))
     .ForMember(d => d.ExpiryDate, opts => opts.MapFrom(s => s.ecer_ExpiryDate))
     .ForMember(d => d.EffectiveDate, opts => opts.MapFrom(s => s.ecer_EffectiveDate))
     .ForMember(d => d.Date, opts => opts.MapFrom(s => s.ecer_Date))
     .ForMember(d => d.PrintDate, opts => opts.MapFrom(s => s.ecer_PrintedDate))
     .ForMember(d => d.HasConditions, opts => opts.MapFrom(s => s.ecer_HasConditions))
     .ForMember(d => d.LevelName, opts => opts.MapFrom(s => s.ecer_CertificateLevel))
     .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.StatusCode))
     .ForMember(d => d.IneligibleReference, opts => opts.MapFrom(s => s.ecer_IneligibleReference))
     .ForMember(d => d.Levels, opts => opts.MapFrom(s => s.ecer_certifiedlevel_CertificateId))
     .ForMember(d => d.Files, opts => opts.MapFrom(s => s.ecer_documenturl_CertificateId));

    CreateMap<ecer_CertifiedLevel, CertificationLevel>(MemberList.Destination)
    .ForMember(d => d.Id, opts => opts.MapFrom(s => s.ecer_CertifiedLevelId))
    .ForMember(d => d.Type, opts => opts.MapFrom(s => s.ecer_CertificateTypeIdName));

    CreateMap<bcgov_DocumentUrl, CertificationFile>(MemberList.Destination)
    .ForMember(d => d.Id, opts => opts.MapFrom(s => s.bcgov_DocumentUrlId))
    .ForMember(d => d.Url, opts => opts.MapFrom(s => s.bcgov_Url))
    .ForMember(d => d.Extention, opts => opts.MapFrom(s => s.bcgov_FileExtension))
    .ForMember(d => d.Size, opts => opts.MapFrom(s => s.bcgov_FileSize))
    .ForMember(d => d.Name, opts => opts.MapFrom(s => s.bcgov_FileName));

    CreateMap<CertificateStatusCode, ecer_Certificate_StatusCode>()
    .ConvertUsingEnumMapping(opts => opts.MapByName(true))
    .ReverseMap();

    CreateMap<YesNoNull, ecer_YesNoNull>()
    .ConvertUsingEnumMapping(opts => opts.MapByName(true))
    .ReverseMap();
  }
}
