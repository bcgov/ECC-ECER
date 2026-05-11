using Riok.Mapperly.Abstractions;
using ContractApplications = ECER.Managers.Registry.Contract.Applications;
using ContractMetadatas = ECER.Managers.Admin.Contract.Metadatas;
using ResourceApplications = ECER.Resources.Documents.Applications;
using ResourceMetadata = ECER.Resources.Documents.MetadataResources;

namespace ECER.Managers.Registry;

public interface IApplicationMapper
{
  ResourceApplications.Application MapApplication(ContractApplications.Application source);
  ContractApplications.Application? MapApplication(ResourceApplications.Application? source);
  IEnumerable<ContractApplications.Application> MapApplications(IEnumerable<ResourceApplications.Application> source);
  ResourceApplications.TranscriptDocuments MapTranscriptDocuments(ContractApplications.TranscriptDocuments source);
  ResourceApplications.CharacterReferenceSubmissionRequest MapCharacterReferenceSubmissionRequest(ContractApplications.CharacterReferenceSubmissionRequest source);
  ResourceApplications.WorkExperienceReferenceSubmissionRequest MapWorkExperienceReferenceSubmissionRequest(ContractApplications.WorkExperienceReferenceSubmissionRequest source);
  ResourceApplications.IcraWorkExperienceReferenceSubmissionRequest MapIcraWorkExperienceReferenceSubmissionRequest(ContractApplications.WorkExperienceReferenceSubmissionRequest source);
  ResourceApplications.OptOutReferenceRequest MapOptOutReferenceRequest(ContractApplications.OptOutReferenceRequest source);
  ResourceApplications.WorkExperienceReference MapWorkExperienceReference(ContractApplications.WorkExperienceReference source);
  ResourceApplications.CharacterReference MapCharacterReference(ContractApplications.CharacterReference source);
  ResourceApplications.ProfessionalDevelopment MapProfessionalDevelopment(ContractApplications.ProfessionalDevelopment source);
}

[Mapper]
internal partial class ApplicationMapper : IApplicationMapper
{
  public ResourceApplications.Application MapApplication(ContractApplications.Application source) => new(
    source.Id,
    source.RegistrantId,
    source.CertificationTypes.Select(MapCertificationType).ToList())
  {
    SignedDate = source.SignedDate,
    SubmittedOn = source.SubmittedOn,
    Stage = source.Stage,
    Transcripts = source.Transcripts.Select(MapTranscript).ToList(),
    ProfessionalDevelopments = source.ProfessionalDevelopments.Select(MapProfessionalDevelopment).ToList(),
    WorkExperienceReferences = source.WorkExperienceReferences.Select(MapWorkExperienceReference).ToList(),
    CharacterReferences = source.CharacterReferences.Select(MapCharacterReference).ToList(),
    SubStatus = MapApplicationStatusReasonDetail(source.SubStatus),
    ReadyForAssessmentDate = source.ReadyForAssessmentDate,
    AddMoreCharacterReference = source.AddMoreCharacterReference,
    AddMoreWorkExperienceReference = source.AddMoreWorkExperienceReference,
    AddMoreProfessionalDevelopment = source.AddMoreProfessionalDevelopment,
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

  public ContractApplications.Application? MapApplication(ResourceApplications.Application? source) => source == null
    ? null
    : new ContractApplications.Application(source.Id, source.ApplicantId, MapApplicationStatus(source.Status))
    {
      SubmittedOn = source.SubmittedOn,
      CreatedOn = source.CreatedOn,
      SignedDate = source.SignedDate,
      CertificationTypes = source.CertificationTypes.Select(MapCertificationType).ToList(),
      Transcripts = source.Transcripts.Select(MapTranscript).ToList(),
      ProfessionalDevelopments = source.ProfessionalDevelopments.Select(MapProfessionalDevelopment).ToList(),
      WorkExperienceReferences = source.WorkExperienceReferences.Select(MapWorkExperienceReference).ToList(),
      CharacterReferences = source.CharacterReferences.Select(MapCharacterReference).ToList(),
      Stage = source.Stage,
      FromCertificate = source.FromCertificate,
      SubStatus = MapApplicationStatusReasonDetail(source.SubStatus),
      ReadyForAssessmentDate = source.ReadyForAssessmentDate,
      AddMoreCharacterReference = source.AddMoreCharacterReference,
      AddMoreWorkExperienceReference = source.AddMoreWorkExperienceReference,
      AddMoreProfessionalDevelopment = source.AddMoreProfessionalDevelopment,
      ApplicationType = MapApplicationType(source.ApplicationType),
      EducationOrigin = MapEducationOrigin(source.EducationOrigin),
      EducationRecognition = MapEducationRecognition(source.EducationRecognition),
      OneYearRenewalExplanationChoice = MapOneYearRenewalExplanation(source.OneYearRenewalExplanationChoice),
      FiveYearRenewalExplanationChoice = MapFiveYearRenewalExplanation(source.FiveYearRenewalExplanationChoice),
      RenewalExplanationOther = source.RenewalExplanationOther,
      Origin = MapApplicationOrigin(source.Origin),
      LabourMobilityCertificateInformation = MapCertificateInformation(source.LabourMobilityCertificateInformation),
    };

  public IEnumerable<ContractApplications.Application> MapApplications(IEnumerable<ResourceApplications.Application> source) => source.Select(application => MapApplication(application)!).ToList();

  public ResourceApplications.TranscriptDocuments MapTranscriptDocuments(ContractApplications.TranscriptDocuments source) => new(source.ApplicationId, source.TranscriptId)
  {
    NewCourseOutlineFiles = source.NewCourseOutlineFiles.ToList(),
    NewProgramConfirmationFiles = source.NewProgramConfirmationFiles.ToList(),
    CourseOutlineOptions = MapCourseOutlineOptions(source.CourseOutlineOptions),
    ComprehensiveReportOptions = MapComprehensiveReportOptions(source.ComprehensiveReportOptions),
    ProgramConfirmationOptions = MapProgramConfirmationOptions(source.ProgramConfirmationOptions),
  };

  public ResourceApplications.CharacterReferenceSubmissionRequest MapCharacterReferenceSubmissionRequest(ContractApplications.CharacterReferenceSubmissionRequest source) => new(
    source.WillProvideReference,
    MapReferenceContactInformation(source.ReferenceContactInformation),
    MapCharacterReferenceEvaluation(source.ReferenceEvaluation),
    source.ConfirmProvidedInformationIsRight);

  public ResourceApplications.WorkExperienceReferenceSubmissionRequest MapWorkExperienceReferenceSubmissionRequest(ContractApplications.WorkExperienceReferenceSubmissionRequest source) => new(
    source.WillProvideReference,
    MapReferenceContactInformation(source.ReferenceContactInformation),
    MapWorkExperienceReferenceDetails(source.WorkExperienceReferenceDetails),
    MapWorkExperienceReferenceCompetenciesAssessment(source.WorkExperienceReferenceCompetenciesAssessment),
    source.ConfirmProvidedInformationIsRight);

  public ResourceApplications.IcraWorkExperienceReferenceSubmissionRequest MapIcraWorkExperienceReferenceSubmissionRequest(ContractApplications.WorkExperienceReferenceSubmissionRequest source) => new(
    source.WillProvideReference,
    MapReferenceContactInformation(source.ReferenceContactInformation),
    MapWorkExperienceReferenceCompetenciesAssessment(source.WorkExperienceReferenceCompetenciesAssessment),
    source.ConfirmProvidedInformationIsRight);

  public ResourceApplications.OptOutReferenceRequest MapOptOutReferenceRequest(ContractApplications.OptOutReferenceRequest source) => new(
    MapUnableToProvideReferenceReason(source.UnabletoProvideReferenceReasons));

  public ResourceApplications.WorkExperienceReference MapWorkExperienceReference(ContractApplications.WorkExperienceReference source) => new(
    source.FirstName,
    source.LastName,
    source.EmailAddress,
    source.Hours)
  {
    Id = source.Id,
    PhoneNumber = source.PhoneNumber,
    Status = MapWorkExperienceReferenceStage(source.Status),
    WillProvideReference = source.WillProvideReference,
    TotalNumberofHoursApproved = source.TotalNumberofHoursApproved,
    TotalNumberofHoursObserved = source.TotalNumberofHoursObserved,
    Type = MapWorkExperienceType(source.Type),
  };

  public ResourceApplications.CharacterReference MapCharacterReference(ContractApplications.CharacterReference source) => new(
    source.FirstName,
    source.LastName,
    source.PhoneNumber,
    source.EmailAddress)
  {
    Id = source.Id,
    Status = MapCharacterReferenceStage(source.Status),
    WillProvideReference = source.WillProvideReference,
  };

  public ResourceApplications.ProfessionalDevelopment MapProfessionalDevelopment(ContractApplications.ProfessionalDevelopment source) => new(
    source.Id,
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
    Status = MapProfessionalDevelopmentStatus(source.Status),
    DeletedFiles = source.DeletedFiles.ToList(),
    NewFiles = source.NewFiles.ToList(),
    Files = source.Files.Select(MapFileInfo).ToList(),
  };

  private ContractApplications.Transcript MapTranscript(ResourceApplications.Transcript source) => new(
    source.Id,
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
    Status = MapTranscriptStage(source.Status),
    StudentMiddleName = source.StudentMiddleName,
    Country = source.Country == null ? null : MapCountry(source.Country),
    Province = MapNullableProvince(source.Province),
    PostSecondaryInstitution = source.PostSecondaryInstitution == null ? null : MapPostSecondaryInstitution(source.PostSecondaryInstitution),
    TranscriptStatusOption = MapTranscriptStatusOption(source.TranscriptStatusOption),
    CourseOutlineReceivedByRegistry = source.CourseOutlineReceivedByRegistry,
    ProgramConfirmationReceivedByRegistry = source.ProgramConfirmationReceivedByRegistry,
    TranscriptReceivedByRegistry = source.TranscriptReceivedByRegistry,
    ComprehensiveReportReceivedByRegistry = source.ComprehensiveReportReceivedByRegistry,
    CourseOutlineFiles = source.CourseOutlineFiles.Select(MapFileInfo).ToList(),
    ProgramConfirmationFiles = source.ProgramConfirmationFiles.Select(MapFileInfo).ToList(),
    CourseOutlineOptions = MapCourseOutlineOptions(source.CourseOutlineOptions),
    ComprehensiveReportOptions = MapComprehensiveReportOptions(source.ComprehensiveReportOptions),
    ProgramConfirmationOptions = MapProgramConfirmationOptions(source.ProgramConfirmationOptions),
  };

  private ResourceApplications.Transcript MapTranscript(ContractApplications.Transcript source) => new(
    source.Id,
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
    Status = MapTranscriptStage(source.Status),
    StudentMiddleName = source.StudentMiddleName,
    Country = source.Country == null ? null : MapCountry(source.Country),
    Province = MapNullableProvince(source.Province),
    PostSecondaryInstitution = source.PostSecondaryInstitution == null ? null : MapPostSecondaryInstitution(source.PostSecondaryInstitution),
    TranscriptStatusOption = MapTranscriptStatusOption(source.TranscriptStatusOption),
    CourseOutlineReceivedByRegistry = source.CourseOutlineReceivedByRegistry,
    ProgramConfirmationReceivedByRegistry = source.ProgramConfirmationReceivedByRegistry,
    TranscriptReceivedByRegistry = source.TranscriptReceivedByRegistry,
    ComprehensiveReportReceivedByRegistry = source.ComprehensiveReportReceivedByRegistry,
    CourseOutlineFiles = source.CourseOutlineFiles.Select(MapFileInfo).ToList(),
    ProgramConfirmationFiles = source.ProgramConfirmationFiles.Select(MapFileInfo).ToList(),
    CourseOutlineOptions = MapCourseOutlineOptions(source.CourseOutlineOptions),
    ComprehensiveReportOptions = MapComprehensiveReportOptions(source.ComprehensiveReportOptions),
    ProgramConfirmationOptions = MapProgramConfirmationOptions(source.ProgramConfirmationOptions),
  };

  private ContractApplications.ProfessionalDevelopment MapProfessionalDevelopment(ResourceApplications.ProfessionalDevelopment source) => new(
    source.Id,
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
    Status = MapProfessionalDevelopmentStatus(source.Status),
    DeletedFiles = source.DeletedFiles.ToList(),
    NewFiles = source.NewFiles.ToList(),
    Files = source.Files.Select(MapFileInfo).ToList(),
  };

  private ContractApplications.WorkExperienceReference MapWorkExperienceReference(ResourceApplications.WorkExperienceReference source) => new(
    source.FirstName,
    source.LastName,
    source.EmailAddress,
    source.Hours)
  {
    Id = source.Id,
    PhoneNumber = source.PhoneNumber,
    Status = MapWorkExperienceReferenceStage(source.Status),
    WillProvideReference = source.WillProvideReference,
    TotalNumberofHoursApproved = source.TotalNumberofHoursApproved,
    TotalNumberofHoursObserved = source.TotalNumberofHoursObserved,
    Type = MapWorkExperienceType(source.Type),
  };

  private ContractApplications.CharacterReference MapCharacterReference(ResourceApplications.CharacterReference source) => new(
    source.FirstName,
    source.LastName,
    source.PhoneNumber,
    source.EmailAddress)
  {
    Id = source.Id,
    Status = MapCharacterReferenceStage(source.Status),
    WillProvideReference = source.WillProvideReference,
  };

  private ResourceApplications.ReferenceContactInformation MapReferenceContactInformation(ContractApplications.ReferenceContactInformation source) => new(
    source.LastName,
    source.FirstName,
    source.Email,
    source.PhoneNumber,
    source.CertificateProvinceOther)
  {
    CertificateProvinceId = source.CertificateProvinceId,
    CertificateNumber = source.CertificateNumber,
    DateOfBirth = source.DateOfBirth,
  };

  private ResourceApplications.CharacterReferenceEvaluation MapCharacterReferenceEvaluation(ContractApplications.CharacterReferenceEvaluation source) => new(
    MapReferenceRelationship(source.ReferenceRelationship),
    source.ReferenceRelationshipOther,
    MapReferenceKnownTime(source.LengthOfAcquaintance),
    source.WorkedWithChildren,
    source.ChildInteractionObservations,
    source.ApplicantTemperamentAssessment);

  private ResourceApplications.WorkExperienceReferenceDetails MapWorkExperienceReferenceDetails(ContractApplications.WorkExperienceReferenceDetails source) => new()
  {
    Hours = source.Hours,
    WorkHoursType = MapWorkHoursType(source.WorkHoursType),
    ChildrenProgramName = source.ChildrenProgramName,
    ChildrenProgramType = MapChildrenProgramType(source.ChildrenProgramType),
    ChildrenProgramTypeOther = source.ChildrenProgramTypeOther,
    ChildcareAgeRanges = source.ChildcareAgeRanges?.Select(MapChildcareAgeRange).ToList(),
    Role = source.Role,
    AgeofChildrenCaredFor = source.AgeofChildrenCaredFor,
    StartDate = source.StartDate,
    EndDate = source.EndDate,
    ReferenceRelationship = MapReferenceRelationship(source.ReferenceRelationship),
    ReferenceRelationshipOther = source.ReferenceRelationshipOther,
    AdditionalComments = source.AdditionalComments,
    WorkExperienceType = MapWorkExperienceType(source.WorkExperienceType),
  };

  private ResourceApplications.WorkExperienceReferenceCompetenciesAssessment MapWorkExperienceReferenceCompetenciesAssessment(ContractApplications.WorkExperienceReferenceCompetenciesAssessment source) => new()
  {
    ChildDevelopment = MapLikertScale(source.ChildDevelopment),
    ChildDevelopmentReason = source.ChildDevelopmentReason,
    ChildGuidance = MapLikertScale(source.ChildGuidance),
    ChildGuidanceReason = source.ChildGuidanceReason,
    HealthSafetyAndNutrition = MapLikertScale(source.HealthSafetyAndNutrition),
    HealthSafetyAndNutritionReason = source.HealthSafetyAndNutritionReason,
    DevelopAnEceCurriculum = MapLikertScale(source.DevelopAnEceCurriculum),
    DevelopAnEceCurriculumReason = source.DevelopAnEceCurriculumReason,
    ImplementAnEceCurriculum = MapLikertScale(source.ImplementAnEceCurriculum),
    ImplementAnEceCurriculumReason = source.ImplementAnEceCurriculumReason,
    FosteringPositiveRelationChild = MapLikertScale(source.FosteringPositiveRelationChild),
    FosteringPositiveRelationChildReason = source.FosteringPositiveRelationChildReason,
    FosteringPositiveRelationFamily = MapLikertScale(source.FosteringPositiveRelationFamily),
    FosteringPositiveRelationFamilyReason = source.FosteringPositiveRelationFamilyReason,
    FosteringPositiveRelationCoworker = MapLikertScale(source.FosteringPositiveRelationCoworker),
    FosteringPositiveRelationCoworkerReason = source.FosteringPositiveRelationCoworkerReason,
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

  private ResourceApplications.CertificateInformation? MapCertificateInformation(ContractApplications.CertificateInformation? source) => source == null
    ? null
    : new ResourceApplications.CertificateInformation
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

  private ContractApplications.CertificateInformation? MapCertificateInformation(ResourceApplications.CertificateInformation? source) => source == null
    ? null
    : new ContractApplications.CertificateInformation
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

  private static ResourceMetadata.Country MapCountry(ContractMetadatas.Country source) => new(source.CountryId, source.CountryName, source.CountryCode, source.IsICRA);

  private static ContractMetadatas.Country MapCountry(ResourceMetadata.Country source) => new(source.CountryId, source.CountryName, source.CountryCode, source.IsICRA);

  private static ResourceMetadata.Province MapProvince(ContractMetadatas.Province source) => new(source.ProvinceId, source.ProvinceName, source.ProvinceCode);

  private static ContractMetadatas.Province MapProvince(ResourceMetadata.Province source) => new(source.ProvinceId, source.ProvinceName, source.ProvinceCode);

  private static ResourceMetadata.Province? MapNullableProvince(ContractMetadatas.Province? source) => source == null ? null : MapProvince(source);

  private static ContractMetadatas.Province? MapNullableProvince(ResourceMetadata.Province? source) => source == null ? null : MapProvince(source);

  private static ResourceMetadata.PostSecondaryInstitution MapPostSecondaryInstitution(ContractMetadatas.PostSecondaryInstitution source) => new(source.Id, source.Name, source.ProvinceId);

  private static ContractMetadatas.PostSecondaryInstitution MapPostSecondaryInstitution(ResourceMetadata.PostSecondaryInstitution source) => new(source.Id, source.Name, source.ProvinceId);

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
  private partial ResourceApplications.ApplicationStatus MapApplicationStatus(ContractApplications.ApplicationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ApplicationStatus MapApplicationStatus(ResourceApplications.ApplicationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.ApplicationStatusReasonDetail MapApplicationStatusReasonDetail(ContractApplications.ApplicationStatusReasonDetail source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ApplicationStatusReasonDetail MapApplicationStatusReasonDetail(ResourceApplications.ApplicationStatusReasonDetail source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.ApplicationTypes MapApplicationType(ContractApplications.ApplicationTypes source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ApplicationTypes MapApplicationType(ResourceApplications.ApplicationTypes source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.EducationOrigin MapEducationOrigin(ContractApplications.EducationOrigin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.EducationOrigin MapEducationOrigin(ResourceApplications.EducationOrigin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.EducationRecognition MapEducationRecognition(ContractApplications.EducationRecognition source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.EducationRecognition MapEducationRecognition(ResourceApplications.EducationRecognition source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.OneYearRenewalexplanations MapOneYearRenewalExplanation(ContractApplications.OneYearRenewalexplanations source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.OneYearRenewalexplanations MapOneYearRenewalExplanation(ResourceApplications.OneYearRenewalexplanations source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.FiveYearRenewalExplanations MapFiveYearRenewalExplanation(ContractApplications.FiveYearRenewalExplanations source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.FiveYearRenewalExplanations MapFiveYearRenewalExplanation(ResourceApplications.FiveYearRenewalExplanations source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.ApplicationOrigin MapApplicationOrigin(ContractApplications.ApplicationOrigin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ApplicationOrigin MapApplicationOrigin(ResourceApplications.ApplicationOrigin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.TranscriptStage MapTranscriptStage(ContractApplications.TranscriptStage source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.TranscriptStage MapTranscriptStage(ResourceApplications.TranscriptStage source);

  private ResourceApplications.TranscriptStage? MapTranscriptStage(ContractApplications.TranscriptStage? source) => source.HasValue ? MapTranscriptStage(source.Value) : null;

  private ContractApplications.TranscriptStage? MapTranscriptStage(ResourceApplications.TranscriptStage? source) => source.HasValue ? MapTranscriptStage(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.ProfessionalDevelopmentStatusCode MapProfessionalDevelopmentStatus(ContractApplications.ProfessionalDevelopmentStatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ProfessionalDevelopmentStatusCode MapProfessionalDevelopmentStatus(ResourceApplications.ProfessionalDevelopmentStatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.CharacterReferenceStage MapCharacterReferenceStage(ContractApplications.CharacterReferenceStage source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.CharacterReferenceStage MapCharacterReferenceStage(ResourceApplications.CharacterReferenceStage source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.WorkExperienceTypes MapWorkExperienceType(ContractApplications.WorkExperienceTypes source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.WorkExperienceTypes MapWorkExperienceType(ResourceApplications.WorkExperienceTypes source);

  private ResourceApplications.WorkExperienceTypes? MapWorkExperienceType(ContractApplications.WorkExperienceTypes? source) => source.HasValue ? MapWorkExperienceType(source.Value) : null;

  private ContractApplications.WorkExperienceTypes? MapWorkExperienceType(ResourceApplications.WorkExperienceTypes? source) => source.HasValue ? MapWorkExperienceType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.UnabletoProvideReferenceReasons MapUnableToProvideReferenceReason(ContractApplications.UnabletoProvideReferenceReasons source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.LikertScale MapLikertScale(ContractApplications.LikertScale source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.ReferenceRelationship MapReferenceRelationship(ContractApplications.ReferenceRelationship source);

  private ResourceApplications.ReferenceRelationship? MapReferenceRelationship(ContractApplications.ReferenceRelationship? source) => source.HasValue ? MapReferenceRelationship(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.ChildrenProgramType MapChildrenProgramType(ContractApplications.ChildrenProgramType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.WorkHoursType MapWorkHoursType(ContractApplications.WorkHoursType source);

  private ResourceApplications.WorkHoursType? MapWorkHoursType(ContractApplications.WorkHoursType? source) => source.HasValue ? MapWorkHoursType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.ChildcareAgeRanges MapChildcareAgeRange(ContractApplications.ChildcareAgeRanges source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.ReferenceKnownTime MapReferenceKnownTime(ContractApplications.ReferenceKnownTime source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.CourseOutlineOptions MapCourseOutlineOptions(ContractApplications.CourseOutlineOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.CourseOutlineOptions MapCourseOutlineOptions(ResourceApplications.CourseOutlineOptions source);

  private ResourceApplications.CourseOutlineOptions? MapCourseOutlineOptions(ContractApplications.CourseOutlineOptions? source) => source.HasValue ? MapCourseOutlineOptions(source.Value) : null;

  private ContractApplications.CourseOutlineOptions? MapCourseOutlineOptions(ResourceApplications.CourseOutlineOptions? source) => source.HasValue ? MapCourseOutlineOptions(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.ComprehensiveReportOptions MapComprehensiveReportOptions(ContractApplications.ComprehensiveReportOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ComprehensiveReportOptions MapComprehensiveReportOptions(ResourceApplications.ComprehensiveReportOptions source);

  private ResourceApplications.ComprehensiveReportOptions? MapComprehensiveReportOptions(ContractApplications.ComprehensiveReportOptions? source) => source.HasValue ? MapComprehensiveReportOptions(source.Value) : null;

  private ContractApplications.ComprehensiveReportOptions? MapComprehensiveReportOptions(ResourceApplications.ComprehensiveReportOptions? source) => source.HasValue ? MapComprehensiveReportOptions(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.ProgramConfirmationOptions MapProgramConfirmationOptions(ContractApplications.ProgramConfirmationOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ProgramConfirmationOptions MapProgramConfirmationOptions(ResourceApplications.ProgramConfirmationOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.TranscriptStatusOptions MapTranscriptStatusOption(ContractApplications.TranscriptStatusOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.TranscriptStatusOptions MapTranscriptStatusOption(ResourceApplications.TranscriptStatusOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceApplications.CertificationType MapCertificationType(ContractApplications.CertificationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.CertificationType MapCertificationType(ResourceApplications.CertificationType source);

  private ResourceApplications.EducationOrigin? MapEducationOrigin(ContractApplications.EducationOrigin? source) => source.HasValue ? MapEducationOrigin(source.Value) : null;

  private ContractApplications.EducationOrigin? MapEducationOrigin(ResourceApplications.EducationOrigin? source) => source.HasValue ? MapEducationOrigin(source.Value) : null;

  private ResourceApplications.EducationRecognition? MapEducationRecognition(ContractApplications.EducationRecognition? source) => source.HasValue ? MapEducationRecognition(source.Value) : null;

  private ContractApplications.EducationRecognition? MapEducationRecognition(ResourceApplications.EducationRecognition? source) => source.HasValue ? MapEducationRecognition(source.Value) : null;

  private ResourceApplications.OneYearRenewalexplanations? MapOneYearRenewalExplanation(ContractApplications.OneYearRenewalexplanations? source) => source.HasValue ? MapOneYearRenewalExplanation(source.Value) : null;

  private ContractApplications.OneYearRenewalexplanations? MapOneYearRenewalExplanation(ResourceApplications.OneYearRenewalexplanations? source) => source.HasValue ? MapOneYearRenewalExplanation(source.Value) : null;

  private ResourceApplications.FiveYearRenewalExplanations? MapFiveYearRenewalExplanation(ContractApplications.FiveYearRenewalExplanations? source) => source.HasValue ? MapFiveYearRenewalExplanation(source.Value) : null;

  private ContractApplications.FiveYearRenewalExplanations? MapFiveYearRenewalExplanation(ResourceApplications.FiveYearRenewalExplanations? source) => source.HasValue ? MapFiveYearRenewalExplanation(source.Value) : null;

  private ResourceApplications.ApplicationOrigin? MapApplicationOrigin(ContractApplications.ApplicationOrigin? source) => source.HasValue ? MapApplicationOrigin(source.Value) : null;

  private ContractApplications.ApplicationOrigin? MapApplicationOrigin(ResourceApplications.ApplicationOrigin? source) => source.HasValue ? MapApplicationOrigin(source.Value) : null;

  private ResourceApplications.ProfessionalDevelopmentStatusCode? MapProfessionalDevelopmentStatus(ContractApplications.ProfessionalDevelopmentStatusCode? source) => source.HasValue ? MapProfessionalDevelopmentStatus(source.Value) : null;

  private ContractApplications.ProfessionalDevelopmentStatusCode? MapProfessionalDevelopmentStatus(ResourceApplications.ProfessionalDevelopmentStatusCode? source) => source.HasValue ? MapProfessionalDevelopmentStatus(source.Value) : null;

  private ResourceApplications.CharacterReferenceStage? MapCharacterReferenceStage(ContractApplications.CharacterReferenceStage? source) => source.HasValue ? MapCharacterReferenceStage(source.Value) : null;

  private ContractApplications.CharacterReferenceStage? MapCharacterReferenceStage(ResourceApplications.CharacterReferenceStage? source) => source.HasValue ? MapCharacterReferenceStage(source.Value) : null;

  private ResourceApplications.LikertScale? MapLikertScale(ContractApplications.LikertScale? source) => source.HasValue ? MapLikertScale(source.Value) : null;

  private ResourceApplications.ChildrenProgramType? MapChildrenProgramType(ContractApplications.ChildrenProgramType? source) => source.HasValue ? MapChildrenProgramType(source.Value) : null;

  private ResourceApplications.ProgramConfirmationOptions? MapProgramConfirmationOptions(ContractApplications.ProgramConfirmationOptions? source) => source.HasValue ? MapProgramConfirmationOptions(source.Value) : null;

  private ContractApplications.ProgramConfirmationOptions? MapProgramConfirmationOptions(ResourceApplications.ProgramConfirmationOptions? source) => source.HasValue ? MapProgramConfirmationOptions(source.Value) : null;

  private ResourceApplications.TranscriptStatusOptions? MapTranscriptStatusOption(ContractApplications.TranscriptStatusOptions? source) => source.HasValue ? MapTranscriptStatusOption(source.Value) : null;

  private ContractApplications.TranscriptStatusOptions? MapTranscriptStatusOption(ResourceApplications.TranscriptStatusOptions? source) => source.HasValue ? MapTranscriptStatusOption(source.Value) : null;
}
