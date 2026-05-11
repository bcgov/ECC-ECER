using Riok.Mapperly.Abstractions;
using System.Diagnostics.CodeAnalysis;
using ContractApplications = ECER.Managers.Registry.Contract.Applications;
using ContractICRA = ECER.Managers.Registry.Contract.ICRA;
using ResourceApplications = ECER.Resources.Documents.Applications;
using ResourceICRA = ECER.Resources.Documents.ICRA;

namespace ECER.Managers.Registry;

[SuppressMessage("Naming", "S101:Types should be named in PascalCase", Justification = "ICRA is a domain acronym used consistently throughout the solution.")]
public interface IICRAEligibilityMapper
{
  ResourceICRA.ICRAEligibility MapEligibility(ContractICRA.ICRAEligibility source);
  ContractICRA.ICRAEligibility? MapEligibility(ResourceICRA.ICRAEligibility? source);
  IEnumerable<ContractICRA.ICRAEligibility> MapEligibilities(IEnumerable<ResourceICRA.ICRAEligibility> source);
  ResourceICRA.EmploymentReference MapEmploymentReference(ContractICRA.EmploymentReference source);
  ContractICRA.EmploymentReference MapEmploymentReference(ResourceICRA.EmploymentReference source);
  ResourceICRA.ICRAWorkExperienceReferenceSubmissionRequest MapIcraWorkExperienceReferenceSubmissionRequest(ContractICRA.ICRAWorkExperienceReferenceSubmissionRequest source);
}

[SuppressMessage("Naming", "S101:Types should be named in PascalCase", Justification = "ICRA is a domain acronym used consistently throughout the solution.")]
[Mapper]
internal partial class ICRAEligibilityMapper : IICRAEligibilityMapper
{
  public ResourceICRA.ICRAEligibility MapEligibility(ContractICRA.ICRAEligibility source) => new()
  {
    Id = source.Id,
    PortalStage = source.PortalStage,
    ApplicantId = source.ApplicantId,
    SignedDate = source.SignedDate,
    CreatedOn = source.CreatedOn,
    Status = MapIcraStatus(source.Status),
    Origin = MapIcraEligibilityOrigin(source.Origin),
    FullLegalName = source.FullLegalName,
    UnderstandAgreesApplication = source.UnderstandAgreesApplication,
    InternationalCertifications = source.InternationalCertifications.Select(MapInternationalCertification).ToList(),
    EmploymentReferences = source.EmploymentReferences.Select(MapEmploymentReference).ToList(),
    AddAdditionalEmploymentExperienceReferences = source.AddAdditionalEmploymentExperienceReferences,
  };

  public ContractICRA.ICRAEligibility? MapEligibility(ResourceICRA.ICRAEligibility? source) => source == null ? null : new ContractICRA.ICRAEligibility
  {
    Id = source.Id,
    PortalStage = source.PortalStage,
    ApplicantId = source.ApplicantId,
    SignedDate = source.SignedDate,
    CreatedOn = source.CreatedOn,
    Status = MapIcraStatus(source.Status),
    Origin = MapIcraEligibilityOrigin(source.Origin),
    FullLegalName = source.FullLegalName,
    UnderstandAgreesApplication = source.UnderstandAgreesApplication,
    InternationalCertifications = source.InternationalCertifications.Select(MapInternationalCertification).ToList(),
    EmploymentReferences = source.EmploymentReferences.Select(MapEmploymentReference).ToList(),
    AddAdditionalEmploymentExperienceReferences = source.AddAdditionalEmploymentExperienceReferences,
  };

  public IEnumerable<ContractICRA.ICRAEligibility> MapEligibilities(IEnumerable<ResourceICRA.ICRAEligibility> source) => source.Select(eligibility => MapEligibility(eligibility)!).ToList();

  public ResourceICRA.EmploymentReference MapEmploymentReference(ContractICRA.EmploymentReference source) => new()
  {
    Id = source.Id,
    LastName = source.LastName,
    FirstName = source.FirstName,
    EmailAddress = source.EmailAddress,
    PhoneNumber = source.PhoneNumber,
    Status = MapWorkExperienceReferenceStage(source.Status),
    WillProvideReference = source.WillProvideReference,
    Type = MapWorkExperienceType(source.Type),
  };

  public ContractICRA.EmploymentReference MapEmploymentReference(ResourceICRA.EmploymentReference source) => new()
  {
    Id = source.Id,
    LastName = source.LastName,
    FirstName = source.FirstName,
    EmailAddress = source.EmailAddress,
    PhoneNumber = source.PhoneNumber,
    Status = MapWorkExperienceReferenceStage(source.Status),
    WillProvideReference = source.WillProvideReference,
    Type = MapWorkExperienceType(source.Type),
  };

  public ResourceICRA.ICRAWorkExperienceReferenceSubmissionRequest MapIcraWorkExperienceReferenceSubmissionRequest(ContractICRA.ICRAWorkExperienceReferenceSubmissionRequest source) => new()
  {
    FirstName = source.FirstName,
    LastName = source.LastName,
    EmailAddress = source.EmailAddress,
    PhoneNumber = source.PhoneNumber,
    CountryId = source.CountryId,
    EmployerName = source.EmployerName,
    PositionTitle = source.PositionTitle,
    StartDate = source.StartDate,
    EndDate = source.EndDate,
    WorkedWithChildren = source.WorkedWithChildren,
    ChildcareAgeRanges = source.ChildcareAgeRanges?.Select(MapChildcareAgeRange).ToList(),
    ReferenceRelationship = MapReferenceRelationship(source.ReferenceRelationship),
    WillProvideReference = source.WillProvideReference,
    DateSigned = source.DateSigned,
  };

  private ResourceICRA.InternationalCertification MapInternationalCertification(ContractICRA.InternationalCertification source) => new()
  {
    Id = source.Id,
    OtherFirstName = source.OtherFirstName,
    OtherMiddleName = source.OtherMiddleName,
    OtherLastName = source.OtherLastName,
    HasOtherName = source.HasOtherName,
    CountryId = source.CountryId,
    NameOfRegulatoryAuthority = source.NameOfRegulatoryAuthority,
    EmailOfRegulatoryAuthority = source.EmailOfRegulatoryAuthority,
    PhoneOfRegulatoryAuthority = source.PhoneOfRegulatoryAuthority,
    WebsiteOfRegulatoryAuthority = source.WebsiteOfRegulatoryAuthority,
    OnlineCertificateValidationToolOfRegulatoryAuthority = source.OnlineCertificateValidationToolOfRegulatoryAuthority,
    CertificateStatus = MapCertificateStatus(source.CertificateStatus),
    CertificateTitle = source.CertificateTitle,
    IssueDate = source.IssueDate,
    ExpiryDate = source.ExpiryDate,
    Files = source.Files.Select(MapFileInfo).ToList(),
    DeletedFiles = source.DeletedFiles.ToList(),
    NewFiles = source.NewFiles.ToList(),
    Status = MapInternationalCertificationStatus(source.status),
  };

  private ContractICRA.InternationalCertification MapInternationalCertification(ResourceICRA.InternationalCertification source) => new()
  {
    Id = source.Id,
    OtherFirstName = source.OtherFirstName,
    OtherMiddleName = source.OtherMiddleName,
    OtherLastName = source.OtherLastName,
    HasOtherName = source.HasOtherName,
    CountryId = source.CountryId,
    NameOfRegulatoryAuthority = source.NameOfRegulatoryAuthority,
    EmailOfRegulatoryAuthority = source.EmailOfRegulatoryAuthority,
    PhoneOfRegulatoryAuthority = source.PhoneOfRegulatoryAuthority,
    WebsiteOfRegulatoryAuthority = source.WebsiteOfRegulatoryAuthority,
    OnlineCertificateValidationToolOfRegulatoryAuthority = source.OnlineCertificateValidationToolOfRegulatoryAuthority,
    CertificateStatus = MapCertificateStatus(source.CertificateStatus),
    CertificateTitle = source.CertificateTitle,
    IssueDate = source.IssueDate,
    ExpiryDate = source.ExpiryDate,
    Files = source.Files.Select(MapFileInfo).ToList(),
    DeletedFiles = source.DeletedFiles.ToList(),
    NewFiles = source.NewFiles.ToList(),
    status = MapInternationalCertificationStatus(source.Status),
  };

  private static ResourceApplications.FileInfo MapFileInfo(ContractApplications.FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
  };

  private static ContractApplications.FileInfo MapFileInfo(ResourceApplications.FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
  };

  private static ContractApplications.WorkExperienceRefStage MapWorkExperienceReferenceStage(ResourceApplications.WorkExperienceRefStage source)
  {
    if (Enum.TryParse<ContractApplications.WorkExperienceRefStage>(source.ToString(), out var mapped))
    {
      return mapped;
    }

    return (ContractApplications.WorkExperienceRefStage)(int)source;
  }

  private static ResourceApplications.WorkExperienceRefStage MapWorkExperienceReferenceStage(ContractApplications.WorkExperienceRefStage source)
  {
    if (Enum.TryParse<ResourceApplications.WorkExperienceRefStage>(source.ToString(), out var mapped))
    {
      return mapped;
    }

    return (ResourceApplications.WorkExperienceRefStage)(int)source;
  }

  private static ResourceApplications.WorkExperienceRefStage? MapWorkExperienceReferenceStage(ContractApplications.WorkExperienceRefStage? source) => source.HasValue ? MapWorkExperienceReferenceStage(source.Value) : null;

  private static ContractApplications.WorkExperienceRefStage? MapWorkExperienceReferenceStage(ResourceApplications.WorkExperienceRefStage? source) => source.HasValue ? MapWorkExperienceReferenceStage(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceICRA.ICRAStatus MapIcraStatus(ContractICRA.ICRAStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractICRA.ICRAStatus MapIcraStatus(ResourceICRA.ICRAStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceICRA.IcraEligibilityOrigin MapIcraEligibilityOrigin(ContractICRA.IcraEligibilityOrigin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractICRA.IcraEligibilityOrigin MapIcraEligibilityOrigin(ResourceICRA.IcraEligibilityOrigin source);

  private ResourceICRA.IcraEligibilityOrigin? MapIcraEligibilityOrigin(ContractICRA.IcraEligibilityOrigin? source) => source.HasValue ? MapIcraEligibilityOrigin(source.Value) : null;

  private ContractICRA.IcraEligibilityOrigin? MapIcraEligibilityOrigin(ResourceICRA.IcraEligibilityOrigin? source) => source.HasValue ? MapIcraEligibilityOrigin(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceICRA.CertificateStatus MapCertificateStatus(ContractICRA.CertificateStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractICRA.CertificateStatus MapCertificateStatus(ResourceICRA.CertificateStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceICRA.InternationalCertificationStatus MapInternationalCertificationStatus(ContractICRA.InternationalCertificationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractICRA.InternationalCertificationStatus MapInternationalCertificationStatus(ResourceICRA.InternationalCertificationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceICRA.WorkExperienceTypesIcra MapWorkExperienceType(ContractICRA.WorkExperienceTypesIcra source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractICRA.WorkExperienceTypesIcra MapWorkExperienceType(ResourceICRA.WorkExperienceTypesIcra source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.ChildcareAgeRanges MapChildcareAgeRange(ContractApplications.ChildcareAgeRanges source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.ReferenceRelationship MapReferenceRelationship(ContractApplications.ReferenceRelationship source);

  private ResourceApplications.ReferenceRelationship? MapReferenceRelationship(ContractApplications.ReferenceRelationship? source) => source.HasValue ? MapReferenceRelationship(source.Value) : null;
}
