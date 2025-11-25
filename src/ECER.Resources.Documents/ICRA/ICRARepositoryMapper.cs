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
      .ForSourceMember(s => s.EmploymentReferences, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.Origin, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_ICRAEligibilityAssessmentId, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.ecer_PortalStage, opts => opts.MapFrom(s => s.PortalStage))
      .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status))
      .ForMember(d => d.ecer_DateSigned, opts => opts.MapFrom(s => s.SignedDate))
      .ForMember(d => d.ecer_ApplicantUnderstandAgreesApplication, opts => opts.MapFrom(s => s.UnderstandAgreesApplication))
      .ForMember(d => d.ecer_ApplicantsFullLegalName, opts => opts.MapFrom(s => s.FullLegalName))
      .ForMember(d => d.ecer_Origin, opts => opts.MapFrom(s => s.Origin))
      .ReverseMap()
      .ForMember(d => d.ApplicantId, opts => opts.MapFrom(s => s.ecer_icraeligibilityassessment_ApplicantId.Id))
      .ForMember(d => d.InternationalCertifications, opts => opts.MapFrom(s => s.ecer_internationalcertification_EligibilityAssessment_ecer_icraeligibilityassessment))
      .ForMember(d => d.EmploymentReferences, opts => opts.MapFrom(s => s.ecer_WorkExperienceRef_ecer_ICRAEligibilityAssessment_ecer_ICRAEligibilityAssessment))
      .ForMember(d => d.CreatedOn, opts => opts.MapFrom(s => s.CreatedOn))
      .ForMember(d => d.Status, o => o.MapFrom(s => s.StatusCode))
      .ForMember(d => d.AddAdditionalEmploymentExperienceReferences, o => o.MapFrom(s => s.ecer_AddAdditionalEmploymentExperienceReferences));

    CreateMap<InternationalCertification, ecer_InternationalCertification>(MemberList.Source)
      .ForSourceMember(s => s.NewFiles, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.DeletedFiles, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.Files, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.CountryId, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.Status, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_OtherFirstName, opts => opts.MapFrom(s => s.OtherFirstName))
      .ForMember(d => d.ecer_OtherMiddleName, opts => opts.MapFrom(s => s.OtherMiddleName))
      .ForMember(d => d.ecer_OtherLastName, opts => opts.MapFrom(s => s.OtherLastName))
      .ForMember(d => d.ecer_CertificateHasOtherName, opts => opts.MapFrom(s => s.HasOtherName))
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
      .ForMember(d => d.HasOtherName, opts => opts.MapFrom(s => s.ecer_CertificateHasOtherName))
      .ForMember(d => d.CountryId, opts => opts.MapFrom(s => s.ecer_CountryId != null ? s.ecer_CountryId.Id.ToString() : null))
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
      .ForMember(d => d.Files, opts => opts.MapFrom(src => src.ecer_bcgov_documenturl_internationalcertificationid.ToList()))
      .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode));

    CreateMap<ecer_ICRAEligibilityAssessment_StatusCode, ICRAStatus>()
      .ConvertUsingEnumMapping(o => o.MapByName(true));

    CreateMap<ICRAStatus, ecer_ICRAEligibilityAssessment_StatusCode>()
        .ConvertUsingEnumMapping(o => o.MapByName(true));

    CreateMap<EmploymentReference, ecer_WorkExperienceRef>(MemberList.Source)
      .ForSourceMember(s => s.Status, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.WillProvideReference, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_WorkExperienceRefId, opts => opts.MapFrom(s => string.IsNullOrEmpty(s.Id) ? null : s.Id))
      .ForMember(d => d.ecer_FirstName, opts => opts.MapFrom(s => s.FirstName))
      .ForMember(d => d.ecer_LastName, opts => opts.MapFrom(s => s.LastName))
      .ForMember(d => d.ecer_EmailAddress, opts => opts.MapFrom(s => s.EmailAddress))
      .ForMember(d => d.ecer_PhoneNumber, opts => opts.MapFrom(s => s.PhoneNumber))
      .ForMember(d => d.ecer_Type, opts => opts.MapFrom(s => s.Type));

    CreateMap<ecer_WorkExperienceRef, EmploymentReference>(MemberList.Destination)
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.ecer_WorkExperienceRefId))
      .ForMember(d => d.FirstName, opts => opts.MapFrom(s => s.ecer_FirstName))
      .ForMember(d => d.LastName, opts => opts.MapFrom(s => s.ecer_LastName))
      .ForMember(d => d.EmailAddress, opts => opts.MapFrom(s => s.ecer_EmailAddress))
      .ForMember(d => d.PhoneNumber, opts => opts.MapFrom(s => s.ecer_PhoneNumber))
      .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode))
      .ForMember(d => d.Type, opts => opts.MapFrom(s => s.ecer_Type))
      .ForMember(d => d.WillProvideReference, opts => opts.MapFrom(s => s.ecer_WillProvideReference.HasValue ? s.ecer_WillProvideReference.Equals(ecer_YesNoNull.Yes) : default(bool?)));

    CreateMap<ICRAWorkExperienceReferenceSubmissionRequest, ecer_WorkExperienceRef>(MemberList.Source)
      .ForSourceMember(s => s.CountryId, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.WorkedWithChildren, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.WillProvideReference, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_referencefirstname, opts => opts.MapFrom(s => s.FirstName))
      .ForMember(d => d.ecer_referencelastname, opts => opts.MapFrom(s => s.LastName))
      .ForMember(d => d.ecer_referenceemailaddress, opts => opts.MapFrom(s => s.EmailAddress))
      .ForMember(d => d.ecer_ReferencePhoneNumber, opts => opts.MapFrom(s => s.PhoneNumber))
      .ForMember(d => d.ecer_NameofEmployer, opts => opts.MapFrom(s => s.EmployerName))
      .ForMember(d => d.ecer_Role, opts => opts.MapFrom(s => s.PositionTitle))
      .ForMember(d => d.ecer_StartDate, opts => opts.MapFrom(s => s.StartDate))
      .ForMember(d => d.ecer_EndDate, opts => opts.MapFrom(s => s.EndDate))
      .ForMember(d => d.ecer_Applicantworkchildren, opts => opts.MapFrom(s => s.WorkedWithChildren.HasValue ? (s.WorkedWithChildren.Value ? ecer_YesNoNull.Yes : ecer_YesNoNull.No) : (ecer_YesNoNull?)null))
      .ForMember(d => d.ecer_ChildcareAgeRangeNew, opts => opts.MapFrom(s => s.ChildcareAgeRanges))
      .ForMember(d => d.ecer_RelationshiptoApplicant, opts => opts.MapFrom(s => s.ReferenceRelationship))
      .ForMember(d => d.ecer_WillProvideReference, opts => opts.MapFrom(s => s.WillProvideReference ? ecer_YesNoNull.Yes : ecer_YesNoNull.No))
      .ForMember(d => d.ecer_DateSigned, opts => opts.MapFrom(s => s.DateSigned));
  }

  public static string IdOrEmpty(EntityReference? reference) =>
      reference != null && reference.Id != Guid.Empty
          ? reference.Id.ToString()
          : string.Empty;
}
