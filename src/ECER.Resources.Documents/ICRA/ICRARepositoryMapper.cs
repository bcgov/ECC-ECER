using ApplicationFileInfo = ECER.Resources.Documents.Applications.FileInfo;
using ECER.Resources.Documents.Applications;
using ECER.Utilities.DataverseSdk.Model;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Documents.ICRA;

internal interface IICRARepositoryMapper
{
  ecer_ICRAEligibilityAssessment MapIcraEligibility(ICRAEligibility source);
  List<ecer_ICRAEligibilityAssessment_StatusCode> MapIcraStatuses(IEnumerable<ICRAStatus> source);
  List<ICRAEligibility> MapIcraEligibilities(IEnumerable<ecer_ICRAEligibilityAssessment> source);
  ecer_Origin? MapOrigin(IcraEligibilityOrigin? source);
  List<ecer_WorkExperienceRef> MapEmploymentReferences(IEnumerable<EmploymentReference> source);
  EmploymentReference MapEmploymentReference(ecer_WorkExperienceRef source);
  ecer_WorkExperienceRef MapEmploymentReference(EmploymentReference source);
  ecer_InternationalCertification MapInternationalCertification(InternationalCertification source);
  void ApplyEmploymentReferenceSubmission(ICRAWorkExperienceReferenceSubmissionRequest source, ecer_WorkExperienceRef destination);
}

[Mapper]
internal partial class ICRARepositoryMapper : IICRARepositoryMapper
{
  public ecer_ICRAEligibilityAssessment MapIcraEligibility(ICRAEligibility source) => new()
  {
    ecer_ICRAEligibilityAssessmentId = string.IsNullOrWhiteSpace(source.Id) ? null : Guid.Parse(source.Id),
    ecer_PortalStage = source.PortalStage,
    StatusCode = MapIcraStatus(source.Status),
    ecer_DateSigned = source.SignedDate,
    ecer_ApplicantUnderstandAgreesApplication = source.UnderstandAgreesApplication,
    ecer_ApplicantsFullLegalName = source.FullLegalName,
    ecer_Origin = MapOrigin(source.Origin),
  };

  public List<ecer_ICRAEligibilityAssessment_StatusCode> MapIcraStatuses(IEnumerable<ICRAStatus> source) => source.Select(MapIcraStatus).ToList();

  public List<ICRAEligibility> MapIcraEligibilities(IEnumerable<ecer_ICRAEligibilityAssessment> source) => source.Select(MapIcraEligibility).ToList();

  public ecer_Origin? MapOrigin(IcraEligibilityOrigin? source) => source.HasValue ? MapOrigin(source.Value) : null;

  public List<ecer_WorkExperienceRef> MapEmploymentReferences(IEnumerable<EmploymentReference> source) => source.Select(MapEmploymentReference).ToList();

  public EmploymentReference MapEmploymentReference(ecer_WorkExperienceRef source) => new()
  {
    Id = source.ecer_WorkExperienceRefId?.ToString(),
    FirstName = source.ecer_FirstName,
    LastName = source.ecer_LastName,
    EmailAddress = source.ecer_EmailAddress,
    PhoneNumber = source.ecer_PhoneNumber,
    Status = source.StatusCode.HasValue ? MapEmploymentReferenceStatus(source.StatusCode.Value) : null,
    Type = source.ecer_Type.HasValue ? MapWorkExperienceType(source.ecer_Type.Value) : WorkExperienceTypesIcra.ICRA,
    WillProvideReference = source.ecer_WillProvideReference.HasValue ? source.ecer_WillProvideReference == ecer_YesNoNull.Yes : null,
  };

  public ecer_WorkExperienceRef MapEmploymentReference(EmploymentReference source) => new()
  {
    ecer_WorkExperienceRefId = string.IsNullOrWhiteSpace(source.Id) ? null : Guid.Parse(source.Id),
    ecer_FirstName = source.FirstName,
    ecer_LastName = source.LastName,
    ecer_EmailAddress = source.EmailAddress,
    ecer_PhoneNumber = source.PhoneNumber,
    ecer_Type = MapWorkExperienceType(source.Type),
  };

  public ecer_InternationalCertification MapInternationalCertification(InternationalCertification source) => new()
  {
    ecer_InternationalCertificationId = string.IsNullOrWhiteSpace(source.Id) ? null : Guid.Parse(source.Id),
    ecer_OtherFirstName = source.OtherFirstName,
    ecer_OtherMiddleName = source.OtherMiddleName,
    ecer_OtherLastName = source.OtherLastName,
    ecer_CertificateHasOtherName = source.HasOtherName,
    ecer_AuthorityName = source.NameOfRegulatoryAuthority,
    ecer_AuthorityEmail = source.EmailOfRegulatoryAuthority,
    ecer_AuthorityPhone = source.PhoneOfRegulatoryAuthority,
    ecer_AuthorityWebsite = source.WebsiteOfRegulatoryAuthority,
    ecer_CertificateValidationTool = source.OnlineCertificateValidationToolOfRegulatoryAuthority,
    ecer_CertificationStatus = MapCertificateStatus(source.CertificateStatus),
    ecer_CertificateTitle = source.CertificateTitle,
    ecer_IssueDate = source.IssueDate,
    ecer_Expirydate = source.ExpiryDate,
  };

  public void ApplyEmploymentReferenceSubmission(ICRAWorkExperienceReferenceSubmissionRequest source, ecer_WorkExperienceRef destination)
  {
    destination.ecer_referencefirstname = source.FirstName;
    destination.ecer_referencelastname = source.LastName;
    destination.ecer_referenceemailaddress = source.EmailAddress;
    destination.ecer_ReferencePhoneNumber = source.PhoneNumber;
    destination.ecer_NameofEmployer = source.EmployerName;
    destination.ecer_Role = source.PositionTitle;
    destination.ecer_StartDate = source.StartDate;
    destination.ecer_EndDate = source.EndDate;
    destination.ecer_Applicantworkchildren = source.WorkedWithChildren.HasValue
      ? source.WorkedWithChildren.Value ? ecer_YesNoNull.Yes : ecer_YesNoNull.No
      : null;
    destination.ecer_ChildcareAgeRangeNew = source.ChildcareAgeRanges?.Select(MapChildcareAgeRange).ToList();
    destination.ecer_RelationshiptoApplicant = MapReferenceRelationship(source.ReferenceRelationship);
    destination.ecer_WillProvideReference = source.WillProvideReference ? ecer_YesNoNull.Yes : ecer_YesNoNull.No;
    destination.ecer_DateSigned = source.DateSigned;
  }

  private ICRAEligibility MapIcraEligibility(ecer_ICRAEligibilityAssessment source) => new()
  {
    Id = source.ecer_ICRAEligibilityAssessmentId?.ToString(),
    PortalStage = source.ecer_PortalStage,
    ApplicantId = source.ecer_icraeligibilityassessment_ApplicantId?.Id.ToString() ?? source.ecer_ApplicantId?.Id.ToString() ?? string.Empty,
    SignedDate = source.ecer_DateSigned,
    CreatedOn = source.CreatedOn,
    Status = source.StatusCode.HasValue ? MapIcraStatus(source.StatusCode.Value) : default,
    Origin = source.ecer_Origin.HasValue ? MapOrigin(source.ecer_Origin.Value) : null,
    FullLegalName = source.ecer_ApplicantsFullLegalName ?? string.Empty,
    UnderstandAgreesApplication = source.ecer_ApplicantUnderstandAgreesApplication.GetValueOrDefault(),
    InternationalCertifications = (source.ecer_internationalcertification_EligibilityAssessment_ecer_icraeligibilityassessment ?? Array.Empty<ecer_InternationalCertification>())
      .Select(MapInternationalCertification)
      .ToList(),
    EmploymentReferences = (source.ecer_WorkExperienceRef_ecer_ICRAEligibilityAssessment_ecer_ICRAEligibilityAssessment ?? Array.Empty<ecer_WorkExperienceRef>())
      .Select(MapEmploymentReference)
      .ToList(),
    AddAdditionalEmploymentExperienceReferences = source.ecer_AddAdditionalEmploymentExperienceReferences.GetValueOrDefault(),
  };

  private InternationalCertification MapInternationalCertification(ecer_InternationalCertification source) => new()
  {
    Id = source.ecer_InternationalCertificationId?.ToString(),
    OtherFirstName = source.ecer_OtherFirstName,
    OtherMiddleName = source.ecer_OtherMiddleName,
    OtherLastName = source.ecer_OtherLastName,
    HasOtherName = source.ecer_CertificateHasOtherName.GetValueOrDefault(),
    CountryId = source.ecer_CountryId?.Id.ToString(),
    NameOfRegulatoryAuthority = source.ecer_AuthorityName,
    EmailOfRegulatoryAuthority = source.ecer_AuthorityEmail,
    PhoneOfRegulatoryAuthority = source.ecer_AuthorityPhone,
    WebsiteOfRegulatoryAuthority = source.ecer_AuthorityWebsite,
    OnlineCertificateValidationToolOfRegulatoryAuthority = source.ecer_CertificateValidationTool,
    CertificateStatus = source.ecer_CertificationStatus.HasValue ? MapCertificateStatus(source.ecer_CertificationStatus.Value) : default,
    CertificateTitle = source.ecer_CertificateTitle,
    IssueDate = source.ecer_IssueDate,
    ExpiryDate = source.ecer_Expirydate,
    Files = (source.ecer_bcgov_documenturl_internationalcertificationid ?? Array.Empty<bcgov_DocumentUrl>())
      .Select(MapFileInfo)
      .ToList(),
    Status = source.StatusCode.HasValue ? MapInternationalCertificationStatus(source.StatusCode.Value) : default,
  };

  private static ApplicationFileInfo MapFileInfo(bcgov_DocumentUrl source) => new(source.bcgov_DocumentUrlId?.ToString() ?? string.Empty)
  {
    Name = source.bcgov_FileName,
    Size = source.bcgov_FileSize,
    Url = source.bcgov_Url,
    Extention = source.bcgov_FileExtension,
  };

  private ecer_ReferenceRelationships? MapReferenceRelationship(ReferenceRelationship? source) => source.HasValue ? MapReferenceRelationship(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ReferenceRelationships MapReferenceRelationship(ReferenceRelationship source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ICRAEligibilityAssessment_StatusCode MapIcraStatus(ICRAStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ICRAStatus MapIcraStatus(ecer_ICRAEligibilityAssessment_StatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_Origin MapOrigin(IcraEligibilityOrigin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial IcraEligibilityOrigin MapOrigin(ecer_Origin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_CertificationStatus MapCertificateStatus(CertificateStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CertificateStatus MapCertificateStatus(ecer_CertificationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial InternationalCertificationStatus MapInternationalCertificationStatus(ecer_InternationalCertification_StatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial WorkExperienceRefStage MapEmploymentReferenceStatus(ecer_WorkExperienceRef_StatusCode source);

  private static ecer_WorkExperienceTypes MapWorkExperienceType(WorkExperienceTypesIcra source) => ecer_WorkExperienceTypes.ICRA;

  private static WorkExperienceTypesIcra MapWorkExperienceType(ecer_WorkExperienceTypes source) => WorkExperienceTypesIcra.ICRA;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ChildcareAgeRange MapChildcareAgeRange(ChildcareAgeRanges source);
}
