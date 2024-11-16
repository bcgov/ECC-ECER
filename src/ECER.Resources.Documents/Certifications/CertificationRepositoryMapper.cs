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
     .ForMember(d => d.Files, opts => opts.MapFrom(s => s.ecer_documenturl_CertificateId))
     .ForMember(d => d.CertificateConditions, opts => opts.MapFrom(s => s.ecer_certificate_Registrantid.ecer_certificateconditions_Registrantid));

    CreateMap<ecer_CertificateConditions, CertificateCondition>(MemberList.Destination)
    .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
    .ForMember(d => d.Name, opts => opts.MapFrom(s => s.ecer_Name))
    .ForMember(d => d.Details, opts => opts.MapFrom(s => s.ecer_Details))
    .ForMember(d => d.StartDate, opts => opts.MapFrom(s => s.ecer_StartDate))
    .ForMember(d => d.EndDate, opts => opts.MapFrom(s => s.ecer_EndDate))
    .ForMember(d => d.DisplayOrder, opts => opts.MapFrom(s => s.ecer_DisplayOrder));

    CreateMap<ecer_CertifiedLevel, CertificationLevel>(MemberList.Destination)
    .ForMember(d => d.Id, opts => opts.MapFrom(s => s.ecer_CertifiedLevelId))
    .ForMember(d => d.Type, opts => opts.MapFrom(s => s.ecer_CertificateTypeIdName));

    CreateMap<bcgov_DocumentUrl, CertificationFile>(MemberList.Destination)
    .ForMember(d => d.Id, opts => opts.MapFrom(s => s.bcgov_DocumentUrlId))
    .ForMember(d => d.Url, opts => opts.MapFrom(s => s.bcgov_Url))
    .ForMember(d => d.Extention, opts => opts.MapFrom(s => s.bcgov_FileExtension))
    .ForMember(d => d.Size, opts => opts.MapFrom(s => s.bcgov_FileSize))
    .ForMember(d => d.Name, opts => opts.MapFrom(s => s.bcgov_FileName))
    .ForMember(d => d.CreatedOn, opts => opts.MapFrom(s => s.CreatedOn))
    .ForMember(d => d.Tag1Name, opts => opts.MapFrom(s => s.bcgov_Tag1IdName));

    CreateMap<CertificateStatusCode, ecer_Certificate_StatusCode>()
    .ConvertUsingEnumMapping(opts => opts.MapByName(true))
    .ReverseMap();

    CreateMap<YesNoNull, ecer_YesNoNull>()
    .ConvertUsingEnumMapping(opts => opts.MapByName(true))
    .ReverseMap();

    CreateMap<ecer_CertificateSummary, CertificationSummary>()
    .ForCtorParam(nameof(CertificationSummary.Id), opt => opt.MapFrom(src => src.ecer_CertificateSummaryId))
    .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.ecer_bcgov_documenturl_CertificateSummaryId != null && src.ecer_bcgov_documenturl_CertificateSummaryId.Count() > 0 ? src.ecer_bcgov_documenturl_CertificateSummaryId.OrderByDescending(f => f.CreatedOn).First().bcgov_FileName : null))
    .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.ecer_bcgov_documenturl_CertificateSummaryId != null && src.ecer_bcgov_documenturl_CertificateSummaryId.Count() > 0 ? src.ecer_bcgov_documenturl_CertificateSummaryId.OrderByDescending(f => f.CreatedOn).First().bcgov_Url : null))
    .ForMember(dest => dest.FileExtention, opt => opt.MapFrom(src => src.ecer_bcgov_documenturl_CertificateSummaryId != null && src.ecer_bcgov_documenturl_CertificateSummaryId.Count() > 0 ? src.ecer_bcgov_documenturl_CertificateSummaryId.OrderByDescending(f => f.CreatedOn).First().bcgov_FileExtension : null))
    .ForMember(dest => dest.FileId, opt => opt.MapFrom(src => src.ecer_bcgov_documenturl_CertificateSummaryId != null && src.ecer_bcgov_documenturl_CertificateSummaryId.Count() > 0 ? src.ecer_bcgov_documenturl_CertificateSummaryId.OrderByDescending(f => f.CreatedOn).First().bcgov_DocumentUrlId : null))
    .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.ecer_bcgov_documenturl_CertificateSummaryId != null && src.ecer_bcgov_documenturl_CertificateSummaryId.Count() > 0 ? src.ecer_bcgov_documenturl_CertificateSummaryId.OrderByDescending(f => f.CreatedOn).First().CreatedOn : null));
  }
}
