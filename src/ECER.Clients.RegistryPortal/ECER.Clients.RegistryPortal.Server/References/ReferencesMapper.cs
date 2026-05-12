using Riok.Mapperly.Abstractions;
using ECER.Clients.RegistryPortal.Server.Applications;
using ContractApplications = ECER.Managers.Registry.Contract.Applications;
using ContractICRA = ECER.Managers.Registry.Contract.ICRA;

namespace ECER.Clients.RegistryPortal.Server.References;

internal interface IReferencesMapper
{
  ContractApplications.CharacterReferenceSubmissionRequest MapCharacterReferenceSubmissionRequest(CharacterReferenceSubmissionRequest source);
  ContractApplications.WorkExperienceReferenceSubmissionRequest MapWorkExperienceReferenceSubmissionRequest(WorkExperienceReferenceSubmissionRequest source);
  ContractApplications.OptOutReferenceRequest MapOptOutReferenceRequest(OptOutReferenceRequest source);
  ContractICRA.ICRAWorkExperienceReferenceSubmissionRequest MapIcraWorkExperienceReferenceSubmissionRequest(ICRAWorkExperienceReferenceSubmissionRequest source);
}

[Mapper]
internal partial class ReferencesMapper : IReferencesMapper
{
  public ContractApplications.CharacterReferenceSubmissionRequest MapCharacterReferenceSubmissionRequest(CharacterReferenceSubmissionRequest source) => new(
    source.Token,
    source.WillProvideReference,
    MapReferenceContactInformation(source.ReferenceContactInformation),
    MapCharacterReferenceEvaluation(source.ReferenceEvaluation),
    source.ConfirmProvidedInformationIsRight);

  public ContractApplications.WorkExperienceReferenceSubmissionRequest MapWorkExperienceReferenceSubmissionRequest(WorkExperienceReferenceSubmissionRequest source) => new(
    source.Token,
    source.WillProvideReference,
    MapReferenceContactInformation(source.ReferenceContactInformation),
    MapWorkExperienceReferenceDetails(source.WorkExperienceReferenceDetails),
    source.WorkExperienceReferenceCompetenciesAssessment == null ? null! : MapWorkExperienceReferenceCompetenciesAssessment(source.WorkExperienceReferenceCompetenciesAssessment),
    source.ConfirmProvidedInformationIsRight);

  public ContractApplications.OptOutReferenceRequest MapOptOutReferenceRequest(OptOutReferenceRequest source) => new(
    source.Token,
    MapUnableToProvideReferenceReason(source.UnabletoProvideReferenceReasons));

  public ContractICRA.ICRAWorkExperienceReferenceSubmissionRequest MapIcraWorkExperienceReferenceSubmissionRequest(ICRAWorkExperienceReferenceSubmissionRequest source) => new()
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

  private ContractApplications.ReferenceContactInformation MapReferenceContactInformation(ReferenceContactInformation source) => new(
    source.LastName,
    source.FirstName!,
    source.Email,
    source.PhoneNumber,
    source.CertificateProvinceOther)
  {
    CertificateProvinceId = source.CertificateProvinceId,
    CertificateNumber = source.CertificateNumber,
    DateOfBirth = source.DateOfBirth,
  };

  private ContractApplications.CharacterReferenceEvaluation MapCharacterReferenceEvaluation(CharacterReferenceEvaluation source) => new(
    MapReferenceRelationship(source.ReferenceRelationship),
    source.ReferenceRelationshipOther,
    MapReferenceKnownTime(source.LengthOfAcquaintance),
    source.WorkedWithChildren,
    source.ChildInteractionObservations,
    source.ApplicantTemperamentAssessment);

  private ContractApplications.WorkExperienceReferenceDetails MapWorkExperienceReferenceDetails(WorkExperienceReferenceDetails source) => new()
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

  private ContractApplications.WorkExperienceReferenceCompetenciesAssessment MapWorkExperienceReferenceCompetenciesAssessment(WorkExperienceReferenceCompetenciesAssessment source) => new()
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

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.UnabletoProvideReferenceReasons MapUnableToProvideReferenceReason(UnabletoProvideReferenceReasons source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ReferenceRelationship MapReferenceRelationship(ReferenceRelationship source);

  private ContractApplications.ReferenceRelationship? MapReferenceRelationship(ReferenceRelationship? source) => source.HasValue ? MapReferenceRelationship(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ReferenceKnownTime MapReferenceKnownTime(ReferenceKnownTime source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.WorkHoursType MapWorkHoursType(WorkHoursType source);

  private ContractApplications.WorkHoursType? MapWorkHoursType(WorkHoursType? source) => source.HasValue ? MapWorkHoursType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ChildrenProgramType MapChildrenProgramType(ChildrenProgramType source);

  private ContractApplications.ChildrenProgramType? MapChildrenProgramType(ChildrenProgramType? source) => source.HasValue ? MapChildrenProgramType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.ChildcareAgeRanges MapChildcareAgeRange(ChildcareAgeRanges source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.WorkExperienceTypes MapWorkExperienceType(WorkExperienceTypes source);

  private ContractApplications.WorkExperienceTypes? MapWorkExperienceType(WorkExperienceTypes? source) => source.HasValue ? MapWorkExperienceType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.LikertScale MapLikertScale(LikertScale source);

  private ContractApplications.LikertScale? MapLikertScale(LikertScale? source) => source.HasValue ? MapLikertScale(source.Value) : null;
}
