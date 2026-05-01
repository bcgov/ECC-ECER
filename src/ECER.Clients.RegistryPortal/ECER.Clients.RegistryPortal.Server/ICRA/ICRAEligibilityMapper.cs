using Riok.Mapperly.Abstractions;
using ContractApplications = ECER.Managers.Registry.Contract.Applications;
using ContractICRA = ECER.Managers.Registry.Contract.ICRA;

namespace ECER.Clients.RegistryPortal.Server.ICRA;

internal interface IICRAEligibilityMapper
{
  ContractICRA.ICRAEligibility MapEligibility(ICRAEligibility source);
  ICRAEligibility MapEligibility(ContractICRA.ICRAEligibility source);
  IEnumerable<ICRAEligibility> MapEligibilities(IEnumerable<ContractICRA.ICRAEligibility> source);
  ICRAEligibilityStatus MapEligibilityStatus(ContractICRA.ICRAEligibility source);
  ContractICRA.EmploymentReference MapEmploymentReference(EmploymentReference source);
  EmploymentReference MapEmploymentReference(ContractICRA.EmploymentReference source);
}

[Mapper]
internal partial class ICRAEligibilityMapper : IICRAEligibilityMapper
{
  public ContractICRA.ICRAEligibility MapEligibility(ICRAEligibility source) => new()
  {
    Id = source.Id,
    ApplicantId = source.ApplicantId,
    PortalStage = source.PortalStage,
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

  public ICRAEligibility MapEligibility(ContractICRA.ICRAEligibility source) => new()
  {
    Id = source.Id,
    ApplicantId = source.ApplicantId,
    PortalStage = source.PortalStage,
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

  public IEnumerable<ICRAEligibility> MapEligibilities(IEnumerable<ContractICRA.ICRAEligibility> source) => source.Select(MapEligibility).ToList();

  public ICRAEligibilityStatus MapEligibilityStatus(ContractICRA.ICRAEligibility source) => new(
    source.Id!,
    source.CreatedOn,
    source.SignedDate,
    MapIcraStatus(source.Status))
  {
    InternationalCertifications = source.InternationalCertifications.Select(MapInternationalCertification).ToList(),
    EmploymentReferencesStatus = source.EmploymentReferences.Select(MapEmploymentReferenceStatus).ToList(),
    AddAdditionalEmploymentExperienceReferences = source.AddAdditionalEmploymentExperienceReferences,
  };

  public ContractICRA.EmploymentReference MapEmploymentReference(EmploymentReference source) => new()
  {
    Id = source.Id,
    LastName = source.LastName,
    FirstName = source.FirstName,
    EmailAddress = source.EmailAddress,
    PhoneNumber = source.PhoneNumber,
    Type = ContractICRA.WorkExperienceTypesIcra.ICRA,
  };

  public EmploymentReference MapEmploymentReference(ContractICRA.EmploymentReference source) => new()
  {
    Id = source.Id,
    LastName = source.LastName,
    FirstName = source.FirstName,
    EmailAddress = source.EmailAddress,
    PhoneNumber = source.PhoneNumber,
    Status = MapEmploymentReferenceStatus(source.Status),
  };

  private InternationalCertification MapInternationalCertification(ContractICRA.InternationalCertification source) => new()
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

  private ContractICRA.InternationalCertification MapInternationalCertification(InternationalCertification source) => new()
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

  private EmploymentReferenceStatus MapEmploymentReferenceStatus(ContractICRA.EmploymentReference source) => new(
    source.Id!,
    MapWorkExperienceRefStage(source.Status),
    source.FirstName,
    source.LastName,
    source.EmailAddress)
  {
    PhoneNumber = source.PhoneNumber,
    WillProvideReference = source.WillProvideReference,
  };

  private static Applications.FileInfo MapFileInfo(ContractApplications.FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
  };

  private static ContractApplications.FileInfo MapFileInfo(Applications.FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
  };

  private static ICRAStatus? MapEmploymentReferenceStatus(ContractApplications.WorkExperienceRefStage? source) => source.HasValue ? (ICRAStatus)(int)source.Value : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ICRAStatus MapIcraStatus(ContractICRA.ICRAStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractICRA.ICRAStatus MapIcraStatus(ICRAStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial IcraEligibilityOrigin MapIcraEligibilityOrigin(ContractICRA.IcraEligibilityOrigin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractICRA.IcraEligibilityOrigin MapIcraEligibilityOrigin(IcraEligibilityOrigin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CertificateStatus MapCertificateStatus(ContractICRA.CertificateStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractICRA.CertificateStatus MapCertificateStatus(CertificateStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial InternationalCertificationStatus MapInternationalCertificationStatus(ContractICRA.InternationalCertificationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractICRA.InternationalCertificationStatus MapInternationalCertificationStatus(InternationalCertificationStatus source);

  private static Applications.WorkExperienceRefStage MapWorkExperienceRefStage(ContractApplications.WorkExperienceRefStage source) => source switch
  {
    ContractApplications.WorkExperienceRefStage.ApplicationSubmitted => Applications.WorkExperienceRefStage.ApplicationSubmitted,
    ContractApplications.WorkExperienceRefStage.Approved => Applications.WorkExperienceRefStage.Approved,
    ContractApplications.WorkExperienceRefStage.Draft => Applications.WorkExperienceRefStage.Draft,
    ContractApplications.WorkExperienceRefStage.InProgress => Applications.WorkExperienceRefStage.InProgress,
    ContractApplications.WorkExperienceRefStage.Rejected => Applications.WorkExperienceRefStage.Rejected,
    ContractApplications.WorkExperienceRefStage.Submitted => Applications.WorkExperienceRefStage.Submitted,
    ContractApplications.WorkExperienceRefStage.UnderReview => Applications.WorkExperienceRefStage.UnderReview,
    ContractApplications.WorkExperienceRefStage.WaitingforResponse => Applications.WorkExperienceRefStage.WaitingforResponse,
    ContractApplications.WorkExperienceRefStage.ICRAEligibilitySubmitted => Applications.WorkExperienceRefStage.ICRAEligibilitySubmitted,
    _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
  };

  private IcraEligibilityOrigin? MapIcraEligibilityOrigin(ContractICRA.IcraEligibilityOrigin? source) => source.HasValue ? MapIcraEligibilityOrigin(source.Value) : null;

  private ContractICRA.IcraEligibilityOrigin? MapIcraEligibilityOrigin(IcraEligibilityOrigin? source) => source.HasValue ? MapIcraEligibilityOrigin(source.Value) : null;

  private Applications.WorkExperienceRefStage? MapWorkExperienceRefStage(ContractApplications.WorkExperienceRefStage? source) => source.HasValue ? MapWorkExperienceRefStage(source.Value) : null;
}
