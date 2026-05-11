using Riok.Mapperly.Abstractions;
using ContractApplications = ECER.Managers.Registry.Contract.Applications;
using ContractMetadatas = ECER.Managers.Admin.Contract.Metadatas;

namespace ECER.Clients.RegistryPortal.Server.Applications;

internal interface IApplicationMapper
{
  ContractApplications.Application MapDraftApplication(DraftApplication source, string registrantId);
  Application MapApplication(ContractApplications.Application source);
  IEnumerable<Application> MapApplications(IEnumerable<ContractApplications.Application> source);
  SubmittedApplicationStatus MapSubmittedApplicationStatus(ContractApplications.Application source);
  ContractApplications.WorkExperienceReference MapWorkExperienceReference(WorkExperienceReference source);
  ContractApplications.CharacterReference MapCharacterReference(CharacterReference source);
  ContractApplications.ProfessionalDevelopment MapProfessionalDevelopment(ProfessionalDevelopment source);
  ContractApplications.TranscriptDocuments MapTranscriptDocuments(TranscriptDocuments source, string registrantId);
}

[Mapper]
internal partial class ApplicationMapper : IApplicationMapper
{
  public ContractApplications.Application MapDraftApplication(DraftApplication source, string registrantId) => new(
    source.Id,
    registrantId,
    ContractApplications.ApplicationStatus.Draft)
  {
    SignedDate = source.SignedDate,
    CertificationTypes = source.CertificationTypes.Select(MapCertificationType).ToList(),
    Transcripts = source.Transcripts.Select(MapTranscript).ToList(),
    WorkExperienceReferences = source.WorkExperienceReferences.Select(MapWorkExperienceReference).ToList(),
    CharacterReferences = source.CharacterReferences.Select(MapCharacterReference).ToList(),
    ProfessionalDevelopments = source.ProfessionalDevelopments.Select(MapProfessionalDevelopment).ToList(),
    Stage = source.Stage,
    FromCertificate = source.FromCertificate,
    ApplicationType = MapApplicationType(source.ApplicationType),
    EducationOrigin = MapEducationOrigin(source.EducationOrigin),
    EducationRecognition = MapEducationRecognition(source.EducationRecognition),
    OneYearRenewalExplanationChoice = MapOneYearRenewalExplanation(source.OneYearRenewalExplanationChoice),
    FiveYearRenewalExplanationChoice = MapFiveYearRenewalExplanation(source.FiveYearRenewalExplanationChoice),
    RenewalExplanationOther = source.RenewalExplanationOther,
    LabourMobilityCertificateInformation = MapCertificateInformation(source.LabourMobilityCertificateInformation),
  };

  public IEnumerable<Application> MapApplications(IEnumerable<ContractApplications.Application> source) => source.Select(MapApplication).ToList();

  public Application MapApplication(ContractApplications.Application source) => new()
  {
    Id = source.Id!,
    CreatedOn = source.CreatedOn.GetValueOrDefault(),
    SubmittedOn = source.SubmittedOn,
    SignedDate = source.SignedDate,
    CertificationTypes = source.CertificationTypes.Select(MapCertificationType).ToList(),
    Transcripts = source.Transcripts.Select(MapTranscript).ToList(),
    WorkExperienceReferences = source.WorkExperienceReferences.Select(MapWorkExperienceReference).ToList(),
    CharacterReferences = source.CharacterReferences.Select(MapCharacterReference).ToList(),
    ProfessionalDevelopments = source.ProfessionalDevelopments.Select(MapProfessionalDevelopment).ToList(),
    Status = MapApplicationStatus(source.Status),
    Stage = source.Stage,
    ApplicationType = MapApplicationType(source.ApplicationType),
    EducationOrigin = MapEducationOrigin(source.EducationOrigin),
    EducationRecognition = MapEducationRecognition(source.EducationRecognition),
    OneYearRenewalExplanationChoice = MapOneYearRenewalExplanation(source.OneYearRenewalExplanationChoice),
    FiveYearRenewalExplanationChoice = MapFiveYearRenewalExplanation(source.FiveYearRenewalExplanationChoice),
    RenewalExplanationOther = source.RenewalExplanationOther,
    FromCertificate = source.FromCertificate,
    Origin = MapApplicationOrigin(source.Origin),
    LabourMobilityCertificateInformation = MapCertificateInformation(source.LabourMobilityCertificateInformation),
  };

  public SubmittedApplicationStatus MapSubmittedApplicationStatus(ContractApplications.Application source) => new(
    source.Id!,
    source.SubmittedOn.GetValueOrDefault(),
    MapApplicationStatus(source.Status),
    MapApplicationStatusReasonDetail(source.SubStatus))
  {
    CertificationTypes = source.CertificationTypes.Select(MapCertificationType).ToList(),
    ReadyForAssessmentDate = source.ReadyForAssessmentDate,
    TranscriptsStatus = source.Transcripts.Select(MapTranscriptStatus).ToList(),
    WorkExperienceReferencesStatus = source.WorkExperienceReferences.Select(MapWorkExperienceReferenceStatus).ToList(),
    CharacterReferencesStatus = source.CharacterReferences.Select(MapCharacterReferenceStatus).ToList(),
    ProfessionalDevelopmentsStatus = source.ProfessionalDevelopments.Select(MapProfessionalDevelopmentSummary).ToList(),
    AddMoreCharacterReference = source.AddMoreCharacterReference,
    AddMoreWorkExperienceReference = source.AddMoreWorkExperienceReference,
    AddMoreProfessionalDevelopment = source.AddMoreProfessionalDevelopment,
    FromCertificate = source.FromCertificate,
    ApplicationType = MapApplicationType(source.ApplicationType),
  };

  public ContractApplications.WorkExperienceReference MapWorkExperienceReference(WorkExperienceReference source) => new(
    source.FirstName,
    source.LastName,
    source.EmailAddress,
    source.Hours)
  {
    Id = source.Id,
    PhoneNumber = source.PhoneNumber,
    Type = MapWorkExperienceType(source.Type),
  };

  public ContractApplications.CharacterReference MapCharacterReference(CharacterReference source) => new(
    source.FirstName,
    source.LastName,
    source.PhoneNumber,
    source.EmailAddress)
  {
    Id = source.Id,
  };

  public ContractApplications.ProfessionalDevelopment MapProfessionalDevelopment(ProfessionalDevelopment source) => new(
    string.IsNullOrEmpty(source.Id) ? null : source.Id,
    source.CourseName,
    source.OrganizationName,
    source.StartDate,
    source.EndDate)
  {
    CourseorWorkshopLink = source.CourseorWorkshopLink,
    OrganizationContactInformation = source.OrganizationContactInformation,
    OrganizationEmailAddress = source.OrganizationEmailAddress,
    InstructorName = source.InstructorName,
    NumberOfHours = source.NumberOfHours,
    DeletedFiles = source.DeletedFiles.ToList(),
    NewFiles = source.NewFiles.ToList(),
    Files = source.Files.Select(MapFileInfo).ToList(),
  };

  public ContractApplications.TranscriptDocuments MapTranscriptDocuments(TranscriptDocuments source, string registrantId) => new(source.ApplicationId, source.TranscriptId)
  {
    NewCourseOutlineFiles = source.NewCourseOutlineFiles.ToList(),
    NewProgramConfirmationFiles = source.NewProgramConfirmationFiles.ToList(),
    CourseOutlineOptions = MapCourseOutlineOptions(source.CourseOutlineOptions),
    ComprehensiveReportOptions = MapComprehensiveReportOptions(source.ComprehensiveReportOptions),
    ProgramConfirmationOptions = MapProgramConfirmationOptions(source.ProgramConfirmationOptions),
    RegistrantId = registrantId,
  };

  private Transcript MapTranscript(ContractApplications.Transcript source) => new(
    source.EducationalInstitutionName,
    source.ProgramName!,
    source.StudentLastName,
    source.StartDate,
    source.EndDate,
    source.IsNameUnverified,
    MapEducationRecognition(source.EducationRecognition),
    MapEducationOrigin(source.EducationOrigin))
  {
    Id = source.Id,
    CampusLocation = source.CampusLocation,
    StudentFirstName = source.StudentFirstName,
    StudentMiddleName = source.StudentMiddleName,
    StudentNumber = source.StudentNumber,
    IsECEAssistant = source.IsECEAssistant,
    TranscriptStatusOption = MapTranscriptStatusOption(source.TranscriptStatusOption),
    Country = source.Country == null ? null : MapCountry(source.Country),
    Province = source.Province == null ? null : MapProvince(source.Province),
    PostSecondaryInstitution = source.PostSecondaryInstitution == null ? null : MapPostSecondaryInstitution(source.PostSecondaryInstitution),
  };

  private ContractApplications.Transcript MapTranscript(Transcript source) => new(
    string.IsNullOrEmpty(source.Id) ? null : source.Id,
    source.EducationalInstitutionName,
    source.ProgramName,
    source.StudentNumber,
    source.StartDate,
    source.EndDate,
    source.IsECEAssistant,
    source.StudentFirstName,
    source.StudentLastName,
    source.IsNameUnverified,
    MapEducationRecognition(source.EducationRecognition),
    MapEducationOrigin(source.EducationOrigin))
  {
    CampusLocation = source.CampusLocation,
    StudentMiddleName = source.StudentMiddleName,
    Country = source.Country == null ? null : MapCountry(source.Country),
    Province = source.Province == null ? null : MapProvince(source.Province),
    PostSecondaryInstitution = source.PostSecondaryInstitution == null ? null : MapPostSecondaryInstitution(source.PostSecondaryInstitution),
    TranscriptStatusOption = MapTranscriptStatusOption(source.TranscriptStatusOption),
  };

  private ProfessionalDevelopment MapProfessionalDevelopment(ContractApplications.ProfessionalDevelopment source) => new(
    source.CourseName!,
    source.OrganizationName!,
    source.StartDate,
    source.EndDate,
    source.NumberOfHours.GetValueOrDefault())
  {
    Id = source.Id,
    OrganizationContactInformation = source.OrganizationContactInformation,
    OrganizationEmailAddress = source.OrganizationEmailAddress,
    InstructorName = source.InstructorName,
    CourseorWorkshopLink = source.CourseorWorkshopLink,
    Status = MapProfessionalDevelopmentStatus(source.Status),
    DeletedFiles = source.DeletedFiles.ToList(),
    NewFiles = source.NewFiles.ToList(),
    Files = source.Files.Select(MapFileInfo).ToList(),
  };

  private WorkExperienceReference MapWorkExperienceReference(ContractApplications.WorkExperienceReference source) => new(
    source.LastName!,
    source.EmailAddress!,
    source.Hours.GetValueOrDefault())
  {
    FirstName = source.FirstName,
    Id = source.Id,
    PhoneNumber = source.PhoneNumber,
    Type = MapWorkExperienceType(source.Type),
  };

  private CharacterReference MapCharacterReference(ContractApplications.CharacterReference source) => new(
    source.LastName!,
    source.PhoneNumber,
    source.EmailAddress!)
  {
    FirstName = source.FirstName,
    Id = source.Id,
  };

  private TranscriptStatus MapTranscriptStatus(ContractApplications.Transcript source) => new(
    source.Id!,
    MapTranscriptStage(source.Status.GetValueOrDefault()),
    source.EducationalInstitutionName!,
    source.ProgramName!)
  {
    TranscriptReceivedByRegistry = source.TranscriptReceivedByRegistry,
    ComprehensiveReportReceivedByRegistry = source.ComprehensiveReportReceivedByRegistry,
    CourseOutlineReceivedByRegistry = source.CourseOutlineReceivedByRegistry,
    ProgramConfirmationReceivedByRegistry = source.ProgramConfirmationReceivedByRegistry,
    CourseOutlineFiles = source.CourseOutlineFiles.Select(MapFileInfo).ToList(),
    ProgramConfirmationFiles = source.ProgramConfirmationFiles.Select(MapFileInfo).ToList(),
    ComprehensiveReportOptions = MapComprehensiveReportOptions(source.ComprehensiveReportOptions),
    CourseOutlineOptions = MapCourseOutlineOptions(source.CourseOutlineOptions),
    Country = source.Country == null ? null : MapCountry(source.Country),
    EducationRecognition = MapEducationRecognition(source.EducationRecognition),
    ProgramConfirmationOptions = MapProgramConfirmationOptions(source.ProgramConfirmationOptions),
  };

  private WorkExperienceReferenceStatus MapWorkExperienceReferenceStatus(ContractApplications.WorkExperienceReference source) => new(
    source.Id!,
    MapWorkExperienceRefStage(source.Status.GetValueOrDefault()),
    source.FirstName!,
    source.LastName!,
    source.EmailAddress!)
  {
    PhoneNumber = source.PhoneNumber,
    TotalNumberofHoursAnticipated = source.Hours,
    TotalNumberofHoursApproved = source.TotalNumberofHoursApproved,
    TotalNumberofHoursObserved = source.TotalNumberofHoursObserved,
    WillProvideReference = source.WillProvideReference,
    Type = MapWorkExperienceType(source.Type),
  };

  private CharacterReferenceStatus MapCharacterReferenceStatus(ContractApplications.CharacterReference source) => new(
    source.Id!,
    MapCharacterReferenceStage(source.Status.GetValueOrDefault()),
    source.FirstName!,
    source.LastName!,
    source.EmailAddress!)
  {
    PhoneNumber = source.PhoneNumber,
    WillProvideReference = source.WillProvideReference,
  };

  private ProfessionalDevelopmentStatus MapProfessionalDevelopmentSummary(ContractApplications.ProfessionalDevelopment source) => new(
    source.Id!,
    source.CourseName!,
    source.NumberOfHours.GetValueOrDefault())
  {
    Status = MapProfessionalDevelopmentStatus(source.Status),
  };

  private static FileInfo MapFileInfo(ContractApplications.FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
  };

  private static ContractApplications.FileInfo MapFileInfo(FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
  };

  private CertificateInformation? MapCertificateInformation(ContractApplications.CertificateInformation? source)
  {
    if (source == null)
    {
      return null;
    }

    return new CertificateInformation
    {
      CertificateComparisonId = source.CertificateComparisonId,
      LabourMobilityProvince = MapNullableProvince(source.LabourMobilityProvince),
      CurrentCertificationNumber = source.CurrentCertificationNumber,
      ExistingCertificationType = source.ExistingCertificationType,
      LegalFirstName = source.LegalFirstName,
      LegalMiddleName = source.LegalMiddleName,
      LegalLastName = source.LegalLastName,
      HasOtherName = source.HasOtherName,
    };
  }

  private ContractApplications.CertificateInformation? MapCertificateInformation(CertificateInformation? source)
  {
    if (source == null)
    {
      return null;
    }

    return new ContractApplications.CertificateInformation
    {
      CertificateComparisonId = source.CertificateComparisonId,
      LabourMobilityProvince = MapNullableProvince(source.LabourMobilityProvince),
      CurrentCertificationNumber = source.CurrentCertificationNumber,
      ExistingCertificationType = source.ExistingCertificationType,
      LegalFirstName = source.LegalFirstName,
      LegalMiddleName = source.LegalMiddleName,
      LegalLastName = source.LegalLastName,
      HasOtherName = source.HasOtherName,
    };
  }

  private partial Country MapCountry(ContractMetadatas.Country source);

  private partial ContractMetadatas.Country MapCountry(Country source);

  private partial Province MapProvince(ContractMetadatas.Province source);

  private partial ContractMetadatas.Province MapProvince(Province source);

  private Province? MapNullableProvince(ContractMetadatas.Province? source) => source == null ? null : MapProvince(source);

  private ContractMetadatas.Province? MapNullableProvince(Province? source) => source == null ? null : MapProvince(source);

  private partial PostSecondaryInstitution MapPostSecondaryInstitution(ContractMetadatas.PostSecondaryInstitution source);

  private partial ContractMetadatas.PostSecondaryInstitution MapPostSecondaryInstitution(PostSecondaryInstitution source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CertificationType MapCertificationType(ContractApplications.CertificationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.CertificationType MapCertificationType(CertificationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ApplicationStatus MapApplicationStatus(ContractApplications.ApplicationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ApplicationStatusReasonDetail MapApplicationStatusReasonDetail(ContractApplications.ApplicationStatusReasonDetail source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ApplicationTypes MapApplicationType(ContractApplications.ApplicationTypes source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ApplicationTypes MapApplicationType(ApplicationTypes source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial EducationOrigin MapEducationOrigin(ContractApplications.EducationOrigin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.EducationOrigin MapEducationOrigin(EducationOrigin source);

  private EducationOrigin? MapEducationOrigin(ContractApplications.EducationOrigin? source) => source.HasValue ? MapEducationOrigin(source.Value) : null;

  private ContractApplications.EducationOrigin? MapEducationOrigin(EducationOrigin? source) => source.HasValue ? MapEducationOrigin(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial EducationRecognition MapEducationRecognition(ContractApplications.EducationRecognition source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.EducationRecognition MapEducationRecognition(EducationRecognition source);

  private EducationRecognition? MapEducationRecognition(ContractApplications.EducationRecognition? source) => source.HasValue ? MapEducationRecognition(source.Value) : null;

  private ContractApplications.EducationRecognition? MapEducationRecognition(EducationRecognition? source) => source.HasValue ? MapEducationRecognition(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial OneYearRenewalexplanations MapOneYearRenewalExplanation(ContractApplications.OneYearRenewalexplanations source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.OneYearRenewalexplanations MapOneYearRenewalExplanation(OneYearRenewalexplanations source);

  private OneYearRenewalexplanations? MapOneYearRenewalExplanation(ContractApplications.OneYearRenewalexplanations? source) => source.HasValue ? MapOneYearRenewalExplanation(source.Value) : null;

  private ContractApplications.OneYearRenewalexplanations? MapOneYearRenewalExplanation(OneYearRenewalexplanations? source) => source.HasValue ? MapOneYearRenewalExplanation(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial FiveYearRenewalExplanations MapFiveYearRenewalExplanation(ContractApplications.FiveYearRenewalExplanations source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.FiveYearRenewalExplanations MapFiveYearRenewalExplanation(FiveYearRenewalExplanations source);

  private FiveYearRenewalExplanations? MapFiveYearRenewalExplanation(ContractApplications.FiveYearRenewalExplanations? source) => source.HasValue ? MapFiveYearRenewalExplanation(source.Value) : null;

  private ContractApplications.FiveYearRenewalExplanations? MapFiveYearRenewalExplanation(FiveYearRenewalExplanations? source) => source.HasValue ? MapFiveYearRenewalExplanation(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ApplicationOrigin MapApplicationOrigin(ContractApplications.ApplicationOrigin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ApplicationOrigin MapApplicationOrigin(ApplicationOrigin source);

  private ApplicationOrigin? MapApplicationOrigin(ContractApplications.ApplicationOrigin? source) => source.HasValue ? MapApplicationOrigin(source.Value) : null;

  private ContractApplications.ApplicationOrigin? MapApplicationOrigin(ApplicationOrigin? source) => source.HasValue ? MapApplicationOrigin(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial TranscriptStage MapTranscriptStage(ContractApplications.TranscriptStage source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ProfessionalDevelopmentStatusCode MapProfessionalDevelopmentStatus(ContractApplications.ProfessionalDevelopmentStatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ProfessionalDevelopmentStatusCode MapProfessionalDevelopmentStatus(ProfessionalDevelopmentStatusCode source);

  private ProfessionalDevelopmentStatusCode? MapProfessionalDevelopmentStatus(ContractApplications.ProfessionalDevelopmentStatusCode? source) => source.HasValue ? MapProfessionalDevelopmentStatus(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CharacterReferenceStage MapCharacterReferenceStage(ContractApplications.CharacterReferenceStage source);

  private static WorkExperienceRefStage MapWorkExperienceRefStage(ContractApplications.WorkExperienceRefStage source) => source switch
  {
    ContractApplications.WorkExperienceRefStage.ApplicationSubmitted => WorkExperienceRefStage.ApplicationSubmitted,
    ContractApplications.WorkExperienceRefStage.Approved => WorkExperienceRefStage.Approved,
    ContractApplications.WorkExperienceRefStage.Draft => WorkExperienceRefStage.Draft,
    ContractApplications.WorkExperienceRefStage.InProgress => WorkExperienceRefStage.InProgress,
    ContractApplications.WorkExperienceRefStage.Rejected => WorkExperienceRefStage.Rejected,
    ContractApplications.WorkExperienceRefStage.Submitted => WorkExperienceRefStage.Submitted,
    ContractApplications.WorkExperienceRefStage.UnderReview => WorkExperienceRefStage.UnderReview,
    ContractApplications.WorkExperienceRefStage.WaitingforResponse => WorkExperienceRefStage.WaitingforResponse,
    ContractApplications.WorkExperienceRefStage.ICRAEligibilitySubmitted => WorkExperienceRefStage.ICRAEligibilitySubmitted,
    _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial WorkExperienceTypes MapWorkExperienceType(ContractApplications.WorkExperienceTypes source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.WorkExperienceTypes MapWorkExperienceType(WorkExperienceTypes source);

  private WorkExperienceTypes? MapWorkExperienceType(ContractApplications.WorkExperienceTypes? source) => source.HasValue ? MapWorkExperienceType(source.Value) : null;

  private ContractApplications.WorkExperienceTypes? MapWorkExperienceType(WorkExperienceTypes? source) => source.HasValue ? MapWorkExperienceType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CourseOutlineOptions MapCourseOutlineOptions(ContractApplications.CourseOutlineOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.CourseOutlineOptions MapCourseOutlineOptions(CourseOutlineOptions source);

  private CourseOutlineOptions? MapCourseOutlineOptions(ContractApplications.CourseOutlineOptions? source) => source.HasValue ? MapCourseOutlineOptions(source.Value) : null;

  private ContractApplications.CourseOutlineOptions? MapCourseOutlineOptions(CourseOutlineOptions? source) => source.HasValue ? MapCourseOutlineOptions(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ComprehensiveReportOptions MapComprehensiveReportOptions(ContractApplications.ComprehensiveReportOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ComprehensiveReportOptions MapComprehensiveReportOptions(ComprehensiveReportOptions source);

  private ComprehensiveReportOptions? MapComprehensiveReportOptions(ContractApplications.ComprehensiveReportOptions? source) => source.HasValue ? MapComprehensiveReportOptions(source.Value) : null;

  private ContractApplications.ComprehensiveReportOptions? MapComprehensiveReportOptions(ComprehensiveReportOptions? source) => source.HasValue ? MapComprehensiveReportOptions(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ProgramConfirmationOptions MapProgramConfirmationOptions(ContractApplications.ProgramConfirmationOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ProgramConfirmationOptions MapProgramConfirmationOptions(ProgramConfirmationOptions source);

  private ProgramConfirmationOptions? MapProgramConfirmationOptions(ContractApplications.ProgramConfirmationOptions? source) => source.HasValue ? MapProgramConfirmationOptions(source.Value) : null;

  private ContractApplications.ProgramConfirmationOptions? MapProgramConfirmationOptions(ProgramConfirmationOptions? source) => source.HasValue ? MapProgramConfirmationOptions(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial TranscriptStatusOptions MapTranscriptStatusOption(ContractApplications.TranscriptStatusOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.TranscriptStatusOptions MapTranscriptStatusOption(TranscriptStatusOptions source);

  private TranscriptStatusOptions? MapTranscriptStatusOption(ContractApplications.TranscriptStatusOptions? source) => source.HasValue ? MapTranscriptStatusOption(source.Value) : null;

  private ContractApplications.TranscriptStatusOptions? MapTranscriptStatusOption(TranscriptStatusOptions? source) => source.HasValue ? MapTranscriptStatusOption(source.Value) : null;
}
