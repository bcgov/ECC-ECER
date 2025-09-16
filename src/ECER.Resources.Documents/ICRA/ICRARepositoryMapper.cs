using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk;

namespace ECER.Resources.Documents.ICRA;

internal class ICRARepositoryMapper : Profile
{
  public ICRARepositoryMapper()
  {
    CreateMap<ICRAEligibility, ecer_ICRAEligibilityAssessment>(MemberList.Source)
      .ForSourceMember(s => s.ApplicantId, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.CreatedOn, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.InternationalCertifications, opts => opts.DoNotValidate())
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.ecer_PortalStage, opts => opts.MapFrom(s => s.PortalStage))
      .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status))
      .ForMember(d => d.ecer_DateSigned, opts => opts.MapFrom(s => s.SignedDate))
      .ReverseMap()
      .ForMember(d => d.ApplicantId, opts => opts.MapFrom(s => s.ecer_icraeligibilityassessment_ApplicantId.Id))
      .ForMember(d => d.InternationalCertifications, opts => opts.MapFrom(s => s.ecer_internationalcertification_EligibilityAssessment_ecer_icraeligibilityassessment))
      .ForMember(d => d.CreatedOn, opts => opts.MapFrom(s => s.CreatedOn));


    CreateMap<InternationalCertification, ecer_InternationalCertification>(MemberList.Source)
      .ForSourceMember(s => s.NewFiles, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.DeletedFiles, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.Files, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.CountryId, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_InternationalCertificationId, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.ecer_OtherFirstName, opts => opts.MapFrom(s => s.OtherFirstName))
      .ForMember(d => d.ecer_OtherMiddleName, opts => opts.MapFrom(s => s.OtherMiddleName))
      .ForMember(d => d.ecer_OtherLastName, opts => opts.MapFrom(s => s.OtherLastName))
      .ForMember(d => d.ecer_AuthorityName, opts => opts.MapFrom(s => s.NameOfRegulatoryAuthority))
      .ForMember(d => d.ecer_AuthorityEmail, opts => opts.MapFrom(s => s.EmailOfRegulatoryAuthority))
      .ForMember(d => d.ecer_AuthorityPhone, opts => opts.MapFrom(s => s.PhoneOfRegulatoryAuthority))
      .ForMember(d => d.ecer_AuthorityWebsite, opts => opts.MapFrom(s => s.WebsiteOfRegulatoryAuthority))
      .ForMember(d => d.ecer_CertificateValidationTool, opts => opts.MapFrom(s => s.OnlineCertificateValidationToolOfRegulatoryAuthority))
      .ForMember(d => d.ecer_CertificationStatus, opts => opts.MapFrom(s => s.CertificateStatus))
      .ForMember(d => d.ecer_CertificateTitle, opts => opts.MapFrom(s => s.CertificateTitle))
      .ForMember(d => d.ecer_IssueDate, opts => opts.MapFrom(s => s.IssueDate))
      .ForMember(d => d.ecer_Expirydate, opts => opts.MapFrom(s => s.ExpiryDate));

    CreateMap<ecer_InternationalCertification, InternationalCertification>(MemberList.Destination)
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.ecer_InternationalCertificationId))
      .ForMember(d => d.OtherFirstName, opts => opts.MapFrom(s => s.ecer_OtherFirstName))
      .ForMember(d => d.OtherMiddleName, opts => opts.MapFrom(s => s.ecer_OtherMiddleName))
      .ForMember(d => d.OtherLastName, opts => opts.MapFrom(s => s.ecer_OtherLastName))
      .ForMember(d => d.CountryId, opts => opts.MapFrom(s => s.ecer_Country != null ? s.ecer_Country.Id.ToString() : null))
      .ForMember(d => d.NameOfRegulatoryAuthority, opts => opts.MapFrom(s => s.ecer_AuthorityName))
      .ForMember(d => d.EmailOfRegulatoryAuthority, opts => opts.MapFrom(s => s.ecer_AuthorityEmail))
      .ForMember(d => d.PhoneOfRegulatoryAuthority, opts => opts.MapFrom(s => s.ecer_AuthorityPhone))
      .ForMember(d => d.WebsiteOfRegulatoryAuthority, opts => opts.MapFrom(s => s.ecer_AuthorityWebsite))
      .ForMember(d => d.OnlineCertificateValidationToolOfRegulatoryAuthority, opts => opts.MapFrom(s => s.ecer_CertificateValidationTool))
      .ForMember(d => d.CertificateStatus, opts => opts.MapFrom(s => s.ecer_CertificationStatus))
      .ForMember(d => d.CertificateTitle, opts => opts.MapFrom(s => s.ecer_CertificateTitle))
      .ForMember(d => d.IssueDate, opts => opts.MapFrom(s => s.ecer_IssueDate))
      .ForMember(d => d.ExpiryDate, opts => opts.MapFrom(s => s.ecer_Expirydate))
      .ForMember(d => d.NewFiles, opts => opts.Ignore())
      .ForMember(d => d.DeletedFiles, opts => opts.Ignore())
      .ForMember(d => d.Files, opts => opts.MapFrom(src => src.ecer_bcgov_documenturl_internationalcertificationid.ToList()));

    CreateMap<ICRAStatus, ecer_ICRAEligibilityAssessment_StatusCode>()
         .ConvertUsingEnumMapping(opts => opts.MapByName(true))
         .ReverseMap();
  }

  public static string IdOrEmpty(EntityReference? reference) =>
      reference != null && reference.Id != Guid.Empty
          ? reference.Id.ToString()  
          : string.Empty;              
}
