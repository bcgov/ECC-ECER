using ECER.Resources.Documents.MetadataResources;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Documents.Applications;

internal interface IApplicationRepositoryMapper
{
  List<Application> MapApplications(IEnumerable<ecer_Application> source);
  List<ecer_Application_StatusCode> MapApplicationStatuses(IEnumerable<ApplicationStatus> source);
  ecer_Application MapApplication(Application source);
  ecer_Origin? MapOrigin(ApplicationOrigin? source);
  List<ecer_Transcript> MapTranscripts(IEnumerable<Transcript> source);
  ecer_ProfessionalDevelopment MapProfessionalDevelopment(ProfessionalDevelopment source);
  List<ecer_WorkExperienceRef> MapWorkExperienceReferences(IEnumerable<WorkExperienceReference> source);
  WorkExperienceReference? MapWorkExperienceReference(ecer_WorkExperienceRef? source);
  ecer_WorkExperienceRef MapWorkExperienceReference(WorkExperienceReference source);
  List<ecer_CharacterReference> MapCharacterReferences(IEnumerable<CharacterReference> source);
  ecer_CharacterReference MapCharacterReference(CharacterReference source);
  void ApplyCharacterReferenceSubmission(CharacterReferenceSubmissionRequest source, ecer_CharacterReference destination);
  void ApplyWorkExperienceReferenceSubmission(WorkExperienceReferenceSubmissionRequest source, ecer_WorkExperienceRef destination);
  void ApplyIcraWorkExperienceReferenceSubmission(IcraWorkExperienceReferenceSubmissionRequest source, ecer_WorkExperienceRef destination);
  void ApplyOptOutReference(OptOutReferenceRequest source, ecer_CharacterReference destination);
  void ApplyOptOutReference(OptOutReferenceRequest source, ecer_WorkExperienceRef destination);
  void ApplyResendCharacterReferenceInvite(ResendReferenceInviteRequest source, ecer_CharacterReference destination);
}

[Mapper]
internal partial class ApplicationRepositoryMapper : IApplicationRepositoryMapper
{
  public List<Application> MapApplications(IEnumerable<ecer_Application> source) => source.Select(MapApplication).ToList();

  public List<ecer_Application_StatusCode> MapApplicationStatuses(IEnumerable<ApplicationStatus> source) => source.Select(MapApplicationStatus).ToList();

  public ecer_Application MapApplication(Application source) => new()
  {
    ecer_ApplicationId = string.IsNullOrWhiteSpace(source.Id) ? null : Guid.Parse(source.Id),
    StatusCode = MapApplicationStatus(source.Status),
    ecer_IsECEAssistant = source.CertificationTypes.Contains(CertificationType.EceAssistant),
    ecer_isECE1YR = source.CertificationTypes.Contains(CertificationType.OneYear),
    ecer_isECE5YR = source.CertificationTypes.Contains(CertificationType.FiveYears),
    ecer_isITE = source.CertificationTypes.Contains(CertificationType.Ite),
    ecer_isSNE = source.CertificationTypes.Contains(CertificationType.Sne),
    ecer_DateSigned = source.SignedDate,
    ecer_PortalStage = source.Stage,
    ecer_Type = MapApplicationType(source.ApplicationType),
    ecer_EducationOrigin = MapEducationOrigin(source.EducationOrigin),
    ecer_EducationRecognition = MapEducationRecognition(source.EducationRecognition),
    ecer_1YRExplanationChoice = MapOneYearRenewalExplanation(source.OneYearRenewalExplanationChoice),
    ecer_5YRExplanationChoice = MapFiveYearRenewalExplanation(source.FiveYearRenewalExplanationChoice),
    ecer_RenewalExplanationOther = source.RenewalExplanationOther,
    ecer_CurrentCertificationNumber = source.LabourMobilityCertificateInformation?.CurrentCertificationNumber ?? string.Empty,
    ecer_lmcerthasothername = source.LabourMobilityCertificateInformation?.HasOtherName == true,
    ecer_LegalFirstName = source.LabourMobilityCertificateInformation?.LegalFirstName ?? string.Empty,
    ecer_LegalMiddleName = source.LabourMobilityCertificateInformation?.LegalMiddleName ?? string.Empty,
    ecer_LegalLastName = source.LabourMobilityCertificateInformation?.LegalLastName ?? string.Empty,
  };

  public ecer_Origin? MapOrigin(ApplicationOrigin? source) => source.HasValue ? MapOrigin(source.Value) : null;

  public List<ecer_Transcript> MapTranscripts(IEnumerable<Transcript> source) => source.Select(MapTranscript).ToList();

  public ecer_ProfessionalDevelopment MapProfessionalDevelopment(ProfessionalDevelopment source) => new()
  {
    ecer_ProfessionalDevelopmentId = string.IsNullOrWhiteSpace(source.Id) ? null : Guid.Parse(source.Id),
    ecer_StartDate = source.StartDate,
    ecer_EndDate = source.EndDate,
    ecer_CourseName = source.CourseName,
    ecer_CourseorWorkshopLink = source.CourseorWorkshopLink,
    ecer_OrganizationName = source.OrganizationName,
    ecer_HostOrganizationContactInformation = source.OrganizationContactInformation,
    ecer_OrganizationEmailAddress = source.OrganizationEmailAddress,
    ecer_InstructorName = source.InstructorName,
    ecer_TotalAnticipatedHours = MapDecimal(source.NumberOfHours),
    StatusCode = MapProfessionalDevelopmentStatus(source.Status),
  };

  public List<ecer_WorkExperienceRef> MapWorkExperienceReferences(IEnumerable<WorkExperienceReference> source) => source.Select(MapWorkExperienceReference).ToList();

  public WorkExperienceReference? MapWorkExperienceReference(ecer_WorkExperienceRef? source) => source == null ? null : MapWorkExperienceReferenceCore(source);

  public ecer_WorkExperienceRef MapWorkExperienceReference(WorkExperienceReference source) => new()
  {
    ecer_WorkExperienceRefId = string.IsNullOrWhiteSpace(source.Id) ? null : Guid.Parse(source.Id),
    ecer_FirstName = source.FirstName,
    ecer_LastName = source.LastName,
    ecer_EmailAddress = source.EmailAddress,
    ecer_PhoneNumber = source.PhoneNumber,
    ecer_TotalNumberofHoursAnticipated = MapDecimal(source.Hours),
    StatusCode = MapWorkExperienceReferenceStage(source.Status),
    ecer_Type = MapWorkExperienceType(source.Type),
  };

  public List<ecer_CharacterReference> MapCharacterReferences(IEnumerable<CharacterReference> source) => source.Select(MapCharacterReference).ToList();

  public ecer_CharacterReference MapCharacterReference(CharacterReference source) => new()
  {
    ecer_CharacterReferenceId = string.IsNullOrWhiteSpace(source.Id) ? null : Guid.Parse(source.Id),
    ecer_FirstName = source.FirstName,
    ecer_LastName = source.LastName,
    ecer_EmailAddress = source.EmailAddress,
    ecer_PhoneNumber = source.PhoneNumber,
    StatusCode = MapCharacterReferenceStage(source.Status),
  };

  public void ApplyCharacterReferenceSubmission(CharacterReferenceSubmissionRequest source, ecer_CharacterReference destination)
  {
    destination.ecer_referencefirstname = source.ReferenceContactInformation.FirstName;
    destination.ecer_referencelastname = source.ReferenceContactInformation.LastName;
    destination.ecer_referencephonenumber = source.ReferenceContactInformation.PhoneNumber;
    destination.ecer_referenceemailaddress = source.ReferenceContactInformation.Email;
    destination.ecer_ReferenceCertifiedProvince = source.ReferenceContactInformation.CertificateProvinceOther;
    destination.ecer_ReferenceCertificationNumber = source.ReferenceContactInformation.CertificateNumber;
    destination.ecer_HaveObservedApplicantwithChildren = source.ReferenceEvaluation.WorkedWithChildren ? ecer_YesNoNull.Yes : ecer_YesNoNull.No;
    destination.ecer_KnownApplicantTimeChoice = MapReferenceKnownTime(source.ReferenceEvaluation.LengthOfAcquaintance);
    destination.ecer_RelationshiptoApplicant = MapReferenceRelationship(source.ReferenceEvaluation.ReferenceRelationship);
    destination.ecer_RelationshipwithApplicantOther = source.ReferenceEvaluation.ReferenceRelationshipOther;
    destination.ecer_ApplicantSituationDescription = source.ReferenceEvaluation.ChildInteractionObservations;
    destination.ecer_ApplicantSuitableReason = source.ReferenceEvaluation.ApplicantTemperamentAssessment;
    destination.ecer_WillProvideReference = source.WillProvideReference ? ecer_YesNoNull.Yes : ecer_YesNoNull.No;
    destination.ecer_DateSigned = source.DateSigned;
  }

  public void ApplyWorkExperienceReferenceSubmission(WorkExperienceReferenceSubmissionRequest source, ecer_WorkExperienceRef destination)
  {
    destination.ecer_referencefirstname = source.ReferenceContactInformation.FirstName;
    destination.ecer_referencelastname = source.ReferenceContactInformation.LastName;
    destination.ecer_ReferencePhoneNumber = source.ReferenceContactInformation.PhoneNumber;
    destination.ecer_referenceemailaddress = source.ReferenceContactInformation.Email;
    destination.ecer_ReferenceECECertifiedProvince = source.ReferenceContactInformation.CertificateProvinceOther;

    if (source.ReferenceContactInformation.CertificateNumber != null)
    {
      destination.ecer_ReferenceECECertificationNumber = source.ReferenceContactInformation.CertificateNumber;
    }

    if (source.ReferenceContactInformation.DateOfBirth.HasValue)
    {
      destination.ecer_ReferenceBirthDate = source.ReferenceContactInformation.DateOfBirth;
    }

    if (source.WorkExperienceReferenceDetails.Hours.HasValue)
    {
      destination.ecer_TotalNumberofHoursObserved = Convert.ToDecimal(source.WorkExperienceReferenceDetails.Hours.Value);
    }

    if (source.WorkExperienceReferenceDetails.WorkHoursType.HasValue)
    {
      destination.ecer_WorkHoursType = MapWorkHoursType(source.WorkExperienceReferenceDetails.WorkHoursType.Value);
    }

    if (source.WorkExperienceReferenceDetails.ChildrenProgramName != null)
    {
      destination.ecer_ChildCareProgramName = source.WorkExperienceReferenceDetails.ChildrenProgramName;
    }

    if (source.WorkExperienceReferenceDetails.ChildrenProgramType.HasValue)
    {
      destination.ecer_TypeofChildrenProgram = MapChildrenProgramType(source.WorkExperienceReferenceDetails.ChildrenProgramType.Value);
    }

    if (source.WorkExperienceReferenceDetails.ChildrenProgramTypeOther != null)
    {
      destination.ecer_OtherChildProgramType = source.WorkExperienceReferenceDetails.ChildrenProgramTypeOther;
    }

    if (source.WorkExperienceReferenceDetails.Role != null)
    {
      destination.ecer_Role = source.WorkExperienceReferenceDetails.Role;
    }

    if (source.WorkExperienceReferenceDetails.AgeofChildrenCaredFor != null)
    {
      destination.ecer_AgeofChildrenCaredFor = source.WorkExperienceReferenceDetails.AgeofChildrenCaredFor;
    }

    if (source.WorkExperienceReferenceDetails.StartDate.HasValue)
    {
      destination.ecer_StartDate = source.WorkExperienceReferenceDetails.StartDate;
    }

    if (source.WorkExperienceReferenceDetails.EndDate.HasValue)
    {
      destination.ecer_EndDate = source.WorkExperienceReferenceDetails.EndDate;
    }

    if (source.WorkExperienceReferenceDetails.ReferenceRelationship.HasValue)
    {
      destination.ecer_RelationshiptoApplicant = MapReferenceRelationship(source.WorkExperienceReferenceDetails.ReferenceRelationship.Value);
    }

    if (source.WorkExperienceReferenceDetails.ReferenceRelationshipOther != null)
    {
      destination.ecer_RelationshiptoApplicantOther = source.WorkExperienceReferenceDetails.ReferenceRelationshipOther;
    }

    if (source.WorkExperienceReferenceDetails.AdditionalComments != null)
    {
      destination.ecer_AdditionalComments = source.WorkExperienceReferenceDetails.AdditionalComments;
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.ChildDevelopment.HasValue)
    {
      destination.ecer_CompetenceChildDevelopment = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.ChildDevelopment.Value);
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.ChildDevelopmentReason != null)
    {
      destination.ecer_CompetenceChildDevelopmentReason = source.WorkExperienceReferenceCompetenciesAssessment.ChildDevelopmentReason;
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.ChildGuidance.HasValue)
    {
      destination.ecer_CompetenceChildGuidance = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.ChildGuidance.Value);
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.ChildGuidanceReason != null)
    {
      destination.ecer_CompetenceChildGuidanceReason = source.WorkExperienceReferenceCompetenciesAssessment.ChildGuidanceReason;
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.HealthSafetyAndNutrition.HasValue)
    {
      destination.ecer_CompetenceHealthSafetyandNutrition = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.HealthSafetyAndNutrition.Value);
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.HealthSafetyAndNutritionReason != null)
    {
      destination.ecer_CompetenceHealthSafetyandNutritionReason = source.WorkExperienceReferenceCompetenciesAssessment.HealthSafetyAndNutritionReason;
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.DevelopAnEceCurriculum.HasValue)
    {
      destination.ecer_CompetenceDevelopanECECurriculum = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.DevelopAnEceCurriculum.Value);
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.DevelopAnEceCurriculumReason != null)
    {
      destination.ecer_CompetenceDevelopanECECurriculumReason = source.WorkExperienceReferenceCompetenciesAssessment.DevelopAnEceCurriculumReason;
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.ImplementAnEceCurriculum.HasValue)
    {
      destination.ecer_CompetenceImplementanECECurriculum = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.ImplementAnEceCurriculum.Value);
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.ImplementAnEceCurriculumReason != null)
    {
      destination.ecer_CompetenceImplementECECurriculumReason = source.WorkExperienceReferenceCompetenciesAssessment.ImplementAnEceCurriculumReason;
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationChild.HasValue)
    {
      destination.ecer_CompetenceFosteringPositiveRelationChild = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationChild.Value);
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationChildReason != null)
    {
      destination.ecer_CompetenceFosteringRelationChildReason = source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationChildReason;
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationFamily.HasValue)
    {
      destination.ecer_CompetenceFosteringPositiveRelationFamily = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationFamily.Value);
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationFamilyReason != null)
    {
      destination.ecer_CompetenceFosteringRelationFamilyReason = source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationFamilyReason;
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationCoworker.HasValue)
    {
      destination.ecer_CompetenceFosteringPositiveRelationCoworker = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationCoworker.Value);
    }

    if (source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationCoworkerReason != null)
    {
      destination.ecer_CompetenceFosteringRelationCoworkerReason = source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationCoworkerReason;
    }

    if (source.WorkExperienceReferenceDetails.ChildcareAgeRanges != null && source.WorkExperienceReferenceDetails.ChildcareAgeRanges.Any())
    {
      destination.ecer_ChildcareAgeRangeNew = source.WorkExperienceReferenceDetails.ChildcareAgeRanges.Select(MapChildcareAgeRange).ToList();
    }

    destination.ecer_WillProvideReference = source.WillProvideReference ? ecer_YesNoNull.Yes : ecer_YesNoNull.No;
    destination.ecer_DateSigned = source.DateSigned;
  }

  public void ApplyIcraWorkExperienceReferenceSubmission(IcraWorkExperienceReferenceSubmissionRequest source, ecer_WorkExperienceRef destination)
  {
    destination.ecer_referencefirstname = source.ReferenceContactInformation.FirstName;
    destination.ecer_referencelastname = source.ReferenceContactInformation.LastName;
    destination.ecer_ReferencePhoneNumber = source.ReferenceContactInformation.PhoneNumber;
    destination.ecer_referenceemailaddress = source.ReferenceContactInformation.Email;
    destination.ecer_CompetenceChildDevelopment = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.ChildDevelopment);
    destination.ecer_CompetenceChildDevelopmentReason = source.WorkExperienceReferenceCompetenciesAssessment.ChildDevelopmentReason;
    destination.ecer_CompetenceChildGuidance = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.ChildGuidance);
    destination.ecer_CompetenceChildGuidanceReason = source.WorkExperienceReferenceCompetenciesAssessment.ChildGuidanceReason;
    destination.ecer_CompetenceHealthSafetyandNutrition = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.HealthSafetyAndNutrition);
    destination.ecer_CompetenceHealthSafetyandNutritionReason = source.WorkExperienceReferenceCompetenciesAssessment.HealthSafetyAndNutritionReason;
    destination.ecer_CompetenceDevelopanECECurriculum = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.DevelopAnEceCurriculum);
    destination.ecer_CompetenceDevelopanECECurriculumReason = source.WorkExperienceReferenceCompetenciesAssessment.DevelopAnEceCurriculumReason;
    destination.ecer_CompetenceImplementanECECurriculum = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.ImplementAnEceCurriculum);
    destination.ecer_CompetenceImplementECECurriculumReason = source.WorkExperienceReferenceCompetenciesAssessment.ImplementAnEceCurriculumReason;
    destination.ecer_CompetenceFosteringPositiveRelationChild = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationChild);
    destination.ecer_CompetenceFosteringRelationChildReason = source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationChildReason;
    destination.ecer_CompetenceFosteringPositiveRelationFamily = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationFamily);
    destination.ecer_CompetenceFosteringRelationFamilyReason = source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationFamilyReason;
    destination.ecer_CompetenceFosteringPositiveRelationCoworker = MapLikertScale(source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationCoworker);
    destination.ecer_CompetenceFosteringRelationCoworkerReason = source.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationCoworkerReason;
    destination.ecer_WillProvideReference = source.WillProvideReference ? ecer_YesNoNull.Yes : ecer_YesNoNull.No;
    destination.ecer_DateSigned = source.DateSigned;
  }

  public void ApplyOptOutReference(OptOutReferenceRequest source, ecer_CharacterReference destination)
  {
    destination.ecer_UnabletoProvideReferenceReason = MapUnableToProvideReferenceReason(source.UnabletoProvideReferenceReasons);
  }

  public void ApplyOptOutReference(OptOutReferenceRequest source, ecer_WorkExperienceRef destination)
  {
    destination.ecer_UnabletoProvideReferenceReason = MapUnableToProvideReferenceReason(source.UnabletoProvideReferenceReasons);
  }

  public void ApplyResendCharacterReferenceInvite(ResendReferenceInviteRequest source, ecer_CharacterReference destination)
  {
    _ = source;
    _ = destination;
  }

  private Application MapApplication(ecer_Application source) => new(
    source.ecer_ApplicationId?.ToString(),
    source.ecer_Applicantid?.Id.ToString() ?? string.Empty,
    GetCertificationTypes(source))
  {
    Status = source.StatusCode.HasValue ? MapApplicationStatus(source.StatusCode.Value) : ApplicationStatus.Draft,
    SubStatus = source.ecer_StatusReasonDetail.HasValue ? MapApplicationStatusReasonDetail(source.ecer_StatusReasonDetail.Value) : default,
    CreatedOn = source.CreatedOn.GetValueOrDefault(),
    SignedDate = source.ecer_DateSigned,
    SubmittedOn = source.ecer_DateSubmitted,
    Transcripts = (source.ecer_transcript_Applicationid ?? Array.Empty<ecer_Transcript>()).Select(MapTranscript).ToList(),
    ProfessionalDevelopments = (source.ecer_ecer_professionaldevelopment_Applicationi ?? Array.Empty<ecer_ProfessionalDevelopment>()).Select(MapProfessionalDevelopment).ToList(),
    WorkExperienceReferences = (source.ecer_workexperienceref_Applicationid_ecer ?? Array.Empty<ecer_WorkExperienceRef>()).Select(MapWorkExperienceReferenceCore).ToList(),
    CharacterReferences = (source.ecer_characterreference_Applicationid ?? Array.Empty<ecer_CharacterReference>()).Select(MapCharacterReference).ToList(),
    ReadyForAssessmentDate = source.ecer_ReadyforAssessmentDate,
    AddMoreCharacterReference = source.ecer_AddMoreCharacterReference,
    AddMoreWorkExperienceReference = source.ecer_AddMoreWorkExperienceReference,
    AddMoreProfessionalDevelopment = source.ecer_AddMoreProfessionalDevelopment,
    Origin = source.ecer_Origin.HasValue ? MapOrigin(source.ecer_Origin.Value) : null,
    LabourMobilityCertificateInformation = MapCertificateInformation(source),
    Stage = string.IsNullOrEmpty(source.ecer_PortalStage) ? "ContactInformation" : source.ecer_PortalStage,
    FromCertificate = IdOrEmpty(source.ecer_FromCertificateId),
    ApplicationType = source.ecer_Type.HasValue ? MapApplicationType(source.ecer_Type.Value) : default,
    EducationOrigin = source.ecer_EducationOrigin.HasValue ? MapEducationOrigin(source.ecer_EducationOrigin.Value) : null,
    EducationRecognition = source.ecer_EducationRecognition.HasValue ? MapEducationRecognition(source.ecer_EducationRecognition.Value) : null,
    OneYearRenewalExplanationChoice = source.ecer_1YRExplanationChoice.HasValue ? MapOneYearRenewalExplanation(source.ecer_1YRExplanationChoice.Value) : null,
    FiveYearRenewalExplanationChoice = source.ecer_5YRExplanationChoice.HasValue ? MapFiveYearRenewalExplanation(source.ecer_5YRExplanationChoice.Value) : null,
    RenewalExplanationOther = source.ecer_RenewalExplanationOther,
  };

  private static List<CertificationType> GetCertificationTypes(ecer_Application source)
  {
    var types = new List<CertificationType>();
    if (source.ecer_IsECEAssistant.GetValueOrDefault()) types.Add(CertificationType.EceAssistant);
    if (source.ecer_isECE1YR.GetValueOrDefault()) types.Add(CertificationType.OneYear);
    if (source.ecer_isECE5YR.GetValueOrDefault()) types.Add(CertificationType.FiveYears);
    if (source.ecer_isITE.GetValueOrDefault()) types.Add(CertificationType.Ite);
    if (source.ecer_isSNE.GetValueOrDefault()) types.Add(CertificationType.Sne);
    return types;
  }

  private CertificateInformation MapCertificateInformation(ecer_Application source) => new()
  {
    CertificateComparisonId = source.ecer_application_certificationcomparisonid?.ecer_certificationcomparisonId?.ToString() ?? string.Empty,
    ExistingCertificationType = source.ecer_application_certificationcomparisonid?.ecer_transferringcertificateName ?? string.Empty,
    CurrentCertificationNumber = source.ecer_CurrentCertificationNumber,
    HasOtherName = source.ecer_lmcerthasothername,
    LegalFirstName = source.ecer_LegalFirstName,
    LegalMiddleName = source.ecer_LegalMiddleName,
    LegalLastName = source.ecer_LegalLastName,
    LabourMobilityProvince = source.ecer_application_lmprovinceid != null ? MapProvince(source.ecer_application_lmprovinceid) : null,
  };

  private ecer_Transcript MapTranscript(Transcript source) => new()
  {
    ecer_TranscriptId = string.IsNullOrWhiteSpace(source.Id) ? null : Guid.Parse(source.Id),
    ecer_StartDate = source.StartDate,
    ecer_EndDate = source.EndDate,
    ecer_ProgramCourseName = source.ProgramName,
    ecer_CampusLocation = source.CampusLocation,
    ecer_StudentFirstName = source.StudentFirstName,
    ecer_StudentMiddleName = source.StudentMiddleName,
    ecer_StudentLastName = source.StudentLastName,
    ecer_StudentNumber = source.StudentNumber,
    ecer_EducationInstitutionFullName = source.EducationalInstitutionName,
    ecer_IsECEAssistant = source.IsECEAssistant,
    //next 3 lines converts enum TranscriptStatusOption to the 3 boolean fields within dynamics
    ecer_DoesECERegistryHaveTranscript = source.TranscriptStatusOption == TranscriptStatusOptions.RegistryHasTranscript,
    ecer_IsOfficialTranscriptRequested = source.TranscriptStatusOption == TranscriptStatusOptions.OfficialTranscriptRequested,
    ecer_mytranscriptwillrequireenglishtranslation = source.TranscriptStatusOption == TranscriptStatusOptions.TranscriptWillRequireEnglishTranslation,
    ecer_IsNameUnverified = source.IsNameUnverified,
    StatusCode = MapTranscriptStage(source.Status),
    ecer_EducationRecognition = MapEducationRecognition(source.EducationRecognition),
    ecer_EducationOrigin = MapEducationOrigin(source.EducationOrigin),
    ecer_transcript_InstituteCountryId = MapCountry(source.Country),
    ecer_transcript_ProvinceId = MapTranscriptProvince(source.Province),
    ecer_transcript_postsecondaryinstitutionid = MapPostSecondaryInstitution(source.PostSecondaryInstitution),
  };

  private Transcript MapTranscript(ecer_Transcript source)
  {
    var country = MapTranscriptCountry(source);
    var province = MapTranscriptProvince(source);
    var provinceId = province?.ProvinceId ?? string.Empty;

    return new Transcript(
      source.ecer_TranscriptId?.ToString(),
      source.ecer_EducationInstitutionFullName,
      source.ecer_ProgramCourseName,
      source.ecer_StudentNumber,
      source.ecer_StartDate.GetValueOrDefault(),
      source.ecer_EndDate.GetValueOrDefault(),
      source.ecer_IsECEAssistant.GetValueOrDefault(),
      source.ecer_StudentFirstName ?? string.Empty,
      source.ecer_StudentLastName ?? string.Empty,
      source.ecer_IsNameUnverified.GetValueOrDefault(),
      source.ecer_EducationRecognition.HasValue ? MapEducationRecognition(source.ecer_EducationRecognition.Value) : default,
      source.ecer_EducationOrigin.HasValue ? MapEducationOrigin(source.ecer_EducationOrigin.Value) : default)
    {
      TranscriptStatusOption = GetTranscriptStatusOption(source),
      CampusLocation = source.ecer_CampusLocation,
      Status = source.StatusCode.HasValue ? MapTranscriptStage(source.StatusCode.Value) : null,
      StudentMiddleName = source.ecer_StudentMiddleName,
      Country = country,
      Province = province,
      PostSecondaryInstitution = MapTranscriptPostSecondaryInstitution(source, provinceId),
      TranscriptReceivedByRegistry = source.ecer_TranscriptReceived,
      ComprehensiveReportReceivedByRegistry = source.ecer_ComprehensiveEvaluationReportReceived,
      CourseOutlineReceivedByRegistry = source.ecer_CourseOutlineReceived,
      ProgramConfirmationReceivedByRegistry = source.ecer_ProgramConfirmationFormReceived,
      ProgramConfirmationFiles = (source.ecer_bcgov_documenturl_TranscriptId ?? Array.Empty<bcgov_DocumentUrl>())
        .Where(file => file.ecer_Tag1 == "Program Confirmation Form")
        .Select(MapFileInfo)
        .ToList(),
      CourseOutlineFiles = (source.ecer_bcgov_documenturl_TranscriptId ?? Array.Empty<bcgov_DocumentUrl>())
        .Where(file => file.ecer_Tag1 == "Course Outline")
        .Select(MapFileInfo)
        .ToList(),
      CourseOutlineOptions = GetCourseOutlineOptions(source),
      ProgramConfirmationOptions = GetProgramConfirmationOptions(source),
      ComprehensiveReportOptions = GetComprehensiveReportOptions(source),
    };
  }

  private ProfessionalDevelopment MapProfessionalDevelopment(ecer_ProfessionalDevelopment source) => new(
    source.ecer_ProfessionalDevelopmentId?.ToString(),
    source.ecer_CourseName,
    source.ecer_OrganizationName,
    source.ecer_StartDate.GetValueOrDefault(),
    source.ecer_EndDate.GetValueOrDefault())
  {
    OrganizationContactInformation = source.ecer_HostOrganizationContactInformation,
    OrganizationEmailAddress = source.ecer_OrganizationEmailAddress,
    InstructorName = source.ecer_InstructorName,
    NumberOfHours = MapDouble(source.ecer_TotalAnticipatedHours),
    Status = source.StatusCode.HasValue ? MapProfessionalDevelopmentStatus(source.StatusCode.Value) : null,
    CourseorWorkshopLink = source.ecer_CourseorWorkshopLink,
    Files = (source.ecer_bcgov_documenturl_ProfessionalDevelopmentId ?? Array.Empty<bcgov_DocumentUrl>()).Select(MapFileInfo).ToList(),
  };

  private WorkExperienceReference MapWorkExperienceReferenceCore(ecer_WorkExperienceRef source) => new(
    source.ecer_FirstName,
    source.ecer_LastName,
    source.ecer_EmailAddress,
    MapInt(source.ecer_TotalNumberofHoursAnticipated))
  {
    TotalNumberofHoursApproved = MapInt(source.ecer_TotalNumberofHoursApproved),
    TotalNumberofHoursObserved = MapInt(source.ecer_TotalNumberofHoursObserved),
    WillProvideReference = source.ecer_WillProvideReference.HasValue ? source.ecer_WillProvideReference == ecer_YesNoNull.Yes : null,
    PhoneNumber = source.ecer_PhoneNumber,
    Id = source.ecer_WorkExperienceRefId?.ToString(),
    Status = source.StatusCode.HasValue ? MapWorkExperienceReferenceStage(source.StatusCode.Value) : null,
    Type = source.ecer_Type.HasValue ? MapWorkExperienceType(source.ecer_Type.Value) : null,
  };

  private CharacterReference MapCharacterReference(ecer_CharacterReference source) => new(
    source.ecer_FirstName,
    source.ecer_LastName,
    source.ecer_PhoneNumber,
    source.ecer_EmailAddress)
  {
    WillProvideReference = source.ecer_WillProvideReference.HasValue ? source.ecer_WillProvideReference == ecer_YesNoNull.Yes : null,
    Id = source.ecer_CharacterReferenceId?.ToString(),
    Status = source.StatusCode.HasValue ? MapCharacterReferenceStage(source.StatusCode.Value) : null,
  };

  private static FileInfo MapFileInfo(bcgov_DocumentUrl source) => new(source.bcgov_DocumentUrlId?.ToString() ?? string.Empty)
  {
    Name = source.bcgov_FileName,
    Size = source.bcgov_FileSize,
    Url = source.bcgov_Url,
    Extention = source.bcgov_FileExtension,
  };

  private static ecer_Country? MapCountry(Country? source)
  {
    if (source == null || string.IsNullOrWhiteSpace(source.CountryId))
    {
      return null;
    }

    return new ecer_Country(Guid.Parse(source.CountryId));
  }

  private static Country MapCountry(ecer_Country source) => new(
    source.ecer_CountryId?.ToString() ?? string.Empty,
    source.ecer_Name ?? string.Empty,
    source.ecer_ShortName ?? string.Empty,
    source.ecer_EligibleforICRA.GetValueOrDefault());

  private static Province MapProvince(ecer_Province source) => new(
    source.ecer_ProvinceId?.ToString() ?? string.Empty,
    source.ecer_Name ?? string.Empty,
    source.ecer_Abbreviation ?? string.Empty);

  private static ecer_Province? MapTranscriptProvince(Province? source)
  {
    if (source == null || string.IsNullOrWhiteSpace(source.ProvinceId))
    {
      return null;
    }

    return new ecer_Province(Guid.Parse(source.ProvinceId));
  }

  private static ecer_PostSecondaryInstitute? MapPostSecondaryInstitution(PostSecondaryInstitution? source)
  {
    if (source == null || string.IsNullOrWhiteSpace(source.Id))
    {
      return null;
    }

    return new ecer_PostSecondaryInstitute(Guid.Parse(source.Id));
  }

  private static PostSecondaryInstitution MapPostSecondaryInstitution(ecer_PostSecondaryInstitute source, string provinceId) => new(
    source.ecer_PostSecondaryInstituteId?.ToString() ?? string.Empty,
    source.ecer_Name ?? string.Empty,
    provinceId);

  private static Country? MapTranscriptCountry(ecer_Transcript source)
  {
    if (source.ecer_transcript_InstituteCountryId != null)
    {
      return MapCountry(source.ecer_transcript_InstituteCountryId);
    }

    if (source.ecer_InstituteCountryId == null)
    {
      return null;
    }

    return new Country(source.ecer_InstituteCountryId.Id.ToString(), source.ecer_InstituteCountryIdName, string.Empty, false);
  }

  private static Province? MapTranscriptProvince(ecer_Transcript source)
  {
    if (source.ecer_transcript_ProvinceId != null)
    {
      return MapProvince(source.ecer_transcript_ProvinceId);
    }

    if (source.ecer_ProvinceId == null)
    {
      return null;
    }

    return new Province(source.ecer_ProvinceId.Id.ToString(), source.ecer_ProvinceIdName, string.Empty);
  }

  private static PostSecondaryInstitution? MapTranscriptPostSecondaryInstitution(ecer_Transcript source, string provinceId)
  {
    if (source.ecer_transcript_postsecondaryinstitutionid != null)
    {
      return MapPostSecondaryInstitution(source.ecer_transcript_postsecondaryinstitutionid, provinceId);
    }

    if (source.ecer_postsecondaryinstitutionid == null)
    {
      return null;
    }

    return new PostSecondaryInstitution(source.ecer_postsecondaryinstitutionid.Id.ToString(), source.ecer_postsecondaryinstitutionidName, provinceId);
  }

  private static TranscriptStatusOptions? GetTranscriptStatusOption(ecer_Transcript source)
  {
    if (source.ecer_DoesECERegistryHaveTranscript == true)
    {
      return TranscriptStatusOptions.RegistryHasTranscript;
    }

    if (source.ecer_IsOfficialTranscriptRequested == true)
    {
      return TranscriptStatusOptions.OfficialTranscriptRequested;
    }

    if (source.ecer_mytranscriptwillrequireenglishtranslation == true)
    {
      return TranscriptStatusOptions.TranscriptWillRequireEnglishTranslation;
    }

    return null;
  }

  private static CourseOutlineOptions? GetCourseOutlineOptions(ecer_Transcript source)
  {
    if (source.ecer_Ihavemycourseoutlinessyllabiandwillupload == true)
    {
      return CourseOutlineOptions.UploadNow;
    }

    if (source.ecer_isECEregistryalreadyhasmycourseoutline == true)
    {
      return CourseOutlineOptions.RegistryAlreadyHas;
    }

    return null;
  }

  private static ProgramConfirmationOptions? GetProgramConfirmationOptions(ecer_Transcript source)
  {
    if (source.ecer_IhavemyProgramConfirmationandwillupload == true)
    {
      return ProgramConfirmationOptions.UploadNow;
    }

    if (source.ecer_isECEregistryhasprogramconfirmation == true)
    {
      return ProgramConfirmationOptions.RegistryAlreadyHas;
    }

    return null;
  }

  private static ComprehensiveReportOptions? GetComprehensiveReportOptions(ecer_Transcript source)
  {
    if (source.ecer_ECERegistryalreadyhasmyComprehensiveReport == true)
    {
      return ComprehensiveReportOptions.RegistryAlreadyHas;
    }

    if (source.ecer_iwishtoapplyforafeewaiver == true)
    {
      return ComprehensiveReportOptions.FeeWaiver;
    }

    if (source.ecer_ihavesubmittedanapplicationtobcits == true)
    {
      return ComprehensiveReportOptions.InternationalCredentialEvaluationService;
    }

    return null;
  }

  private static decimal? MapDecimal(double? source) => source.HasValue ? Convert.ToDecimal(source.Value) : null;

  private static decimal? MapDecimal(int? source) => source.HasValue ? Convert.ToDecimal(source.Value) : null;

  private static double? MapDouble(decimal? source) => source.HasValue ? Convert.ToDouble(source.Value) : null;

  private static int? MapInt(decimal? source) => source.HasValue ? Convert.ToInt32(source.Value) : null;

  public static string IdOrEmpty(EntityReference? reference) =>
    reference != null && reference.Id != Guid.Empty
      ? reference.Id.ToString()
      : string.Empty;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_Origin MapOrigin(ApplicationOrigin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ApplicationOrigin MapOrigin(ecer_Origin source);

  private static ecer_Application_StatusCode MapApplicationStatus(ApplicationStatus source) => source switch
  {
    ApplicationStatus.Draft => ecer_Application_StatusCode.Draft,
    ApplicationStatus.Submitted => ecer_Application_StatusCode.Submitted,
    ApplicationStatus.Complete => ecer_Application_StatusCode.Complete,
    ApplicationStatus.Closed => ecer_Application_StatusCode.Closed,
    ApplicationStatus.Reconsideration => ecer_Application_StatusCode.Reconsideration,
    ApplicationStatus.Escalated => ecer_Application_StatusCode.Escalated,
    ApplicationStatus.Decision => ecer_Application_StatusCode.Decision,
    ApplicationStatus.Cancelled => ecer_Application_StatusCode.Withdrawn,
    ApplicationStatus.Pending => ecer_Application_StatusCode.Pending,
    ApplicationStatus.Ready => ecer_Application_StatusCode.Ready,
    ApplicationStatus.InProgress => ecer_Application_StatusCode.InProgress,
    ApplicationStatus.PendingQueue => ecer_Application_StatusCode.PendingQueue,
    ApplicationStatus.PendingPSPConsultationNeeded => ecer_Application_StatusCode.PendingPSPConsultationNeeded,
    ApplicationStatus.ReconsiderationDecision => ecer_Application_StatusCode.ReconsiderationDecision,
    ApplicationStatus.AppealDecision => ecer_Application_StatusCode.AppealDecision,
    ApplicationStatus.NotSubmitted => ecer_Application_StatusCode.NotSubmitted,
    _ => ecer_Application_StatusCode.Draft,
  };

  private static ApplicationStatus MapApplicationStatus(ecer_Application_StatusCode source) => source switch
  {
    ecer_Application_StatusCode.Draft => ApplicationStatus.Draft,
    ecer_Application_StatusCode.Submitted => ApplicationStatus.Submitted,
    ecer_Application_StatusCode.Complete => ApplicationStatus.Complete,
    ecer_Application_StatusCode.Closed => ApplicationStatus.Closed,
    ecer_Application_StatusCode.Reconsideration => ApplicationStatus.Reconsideration,
    ecer_Application_StatusCode.Escalated => ApplicationStatus.Escalated,
    ecer_Application_StatusCode.Decision => ApplicationStatus.Decision,
    ecer_Application_StatusCode.Withdrawn => ApplicationStatus.Cancelled,
    ecer_Application_StatusCode.Pending => ApplicationStatus.Pending,
    ecer_Application_StatusCode.Ready => ApplicationStatus.Ready,
    ecer_Application_StatusCode.InProgress => ApplicationStatus.InProgress,
    ecer_Application_StatusCode.PendingQueue => ApplicationStatus.PendingQueue,
    ecer_Application_StatusCode.PendingPSPConsultationNeeded => ApplicationStatus.PendingPSPConsultationNeeded,
    ecer_Application_StatusCode.ReconsiderationDecision => ApplicationStatus.ReconsiderationDecision,
    ecer_Application_StatusCode.AppealDecision => ApplicationStatus.AppealDecision,
    ecer_Application_StatusCode.NotSubmitted => ApplicationStatus.NotSubmitted,
    _ => ApplicationStatus.Draft,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ApplicationStatusReasonDetail MapApplicationStatusReasonDetail(ApplicationStatusReasonDetail source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ApplicationStatusReasonDetail MapApplicationStatusReasonDetail(ecer_ApplicationStatusReasonDetail source);

  private ecer_ApplicationStatusReasonDetail? MapApplicationStatusReasonDetail(ApplicationStatusReasonDetail? source) => source.HasValue ? MapApplicationStatusReasonDetail(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_yrrenewalexplanations MapOneYearRenewalExplanation(OneYearRenewalexplanations source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial OneYearRenewalexplanations MapOneYearRenewalExplanation(ecer_yrrenewalexplanations source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_fiveyrrenewalexplanations MapFiveYearRenewalExplanation(FiveYearRenewalExplanations source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial FiveYearRenewalExplanations MapFiveYearRenewalExplanation(ecer_fiveyrrenewalexplanations source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ApplicationTypes MapApplicationType(ApplicationTypes source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ApplicationTypes MapApplicationType(ecer_ApplicationTypes source);

  private ecer_ApplicationTypes? MapApplicationType(ApplicationTypes? source) => source.HasValue ? MapApplicationType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_EducationOrigin MapEducationOrigin(EducationOrigin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial EducationOrigin MapEducationOrigin(ecer_EducationOrigin source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_EducationRecognition MapEducationRecognition(EducationRecognition source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial EducationRecognition MapEducationRecognition(ecer_EducationRecognition source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_Transcript_StatusCode MapTranscriptStage(TranscriptStage source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial TranscriptStage MapTranscriptStage(ecer_Transcript_StatusCode source);

  private ecer_Transcript_StatusCode? MapTranscriptStage(TranscriptStage? source) => source.HasValue ? MapTranscriptStage(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ProfessionalDevelopment_StatusCode MapProfessionalDevelopmentStatus(ProfessionalDevelopmentStatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ProfessionalDevelopmentStatusCode MapProfessionalDevelopmentStatus(ecer_ProfessionalDevelopment_StatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_CharacterReference_StatusCode MapCharacterReferenceStage(CharacterReferenceStage source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CharacterReferenceStage MapCharacterReferenceStage(ecer_CharacterReference_StatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_WorkExperienceRef_StatusCode MapWorkExperienceReferenceStage(WorkExperienceRefStage source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial WorkExperienceRefStage MapWorkExperienceReferenceStage(ecer_WorkExperienceRef_StatusCode source);

  private ecer_WorkExperienceRef_StatusCode? MapWorkExperienceReferenceStage(WorkExperienceRefStage? source) => source.HasValue ? MapWorkExperienceReferenceStage(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_UnabletoProvideReferenceReasons MapUnableToProvideReferenceReason(UnabletoProvideReferenceReasons source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial UnabletoProvideReferenceReasons MapUnableToProvideReferenceReason(ecer_UnabletoProvideReferenceReasons source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_likertscales MapLikertScale(LikertScale source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial LikertScale MapLikertScale(ecer_likertscales source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ReferenceRelationships MapReferenceRelationship(ReferenceRelationship source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ReferenceRelationship MapReferenceRelationship(ecer_ReferenceRelationships source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ChildrenProgramTypes MapChildrenProgramType(ChildrenProgramType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ChildrenProgramType MapChildrenProgramType(ecer_ChildrenProgramTypes source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_WorkHoursType MapWorkHoursType(WorkHoursType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial WorkHoursType MapWorkHoursType(ecer_WorkHoursType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ChildcareAgeRange MapChildcareAgeRange(ChildcareAgeRanges source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ChildcareAgeRanges MapChildcareAgeRange(ecer_ChildcareAgeRange source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ReferenceKnownTime MapReferenceKnownTime(ReferenceKnownTime source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ReferenceKnownTime MapReferenceKnownTime(ecer_ReferenceKnownTime source);

  private ecer_yrrenewalexplanations? MapOneYearRenewalExplanation(OneYearRenewalexplanations? source) => source.HasValue ? MapOneYearRenewalExplanation(source.Value) : null;

  private ecer_fiveyrrenewalexplanations? MapFiveYearRenewalExplanation(FiveYearRenewalExplanations? source) => source.HasValue ? MapFiveYearRenewalExplanation(source.Value) : null;

  private ecer_EducationOrigin? MapEducationOrigin(EducationOrigin? source) => source.HasValue ? MapEducationOrigin(source.Value) : null;

  private ecer_EducationRecognition? MapEducationRecognition(EducationRecognition? source) => source.HasValue ? MapEducationRecognition(source.Value) : null;

  private ecer_ProfessionalDevelopment_StatusCode? MapProfessionalDevelopmentStatus(ProfessionalDevelopmentStatusCode? source) => source.HasValue ? MapProfessionalDevelopmentStatus(source.Value) : null;

  private ecer_CharacterReference_StatusCode? MapCharacterReferenceStage(CharacterReferenceStage? source) => source.HasValue ? MapCharacterReferenceStage(source.Value) : null;

  private ecer_UnabletoProvideReferenceReasons? MapUnableToProvideReferenceReason(UnabletoProvideReferenceReasons? source) => source.HasValue ? MapUnableToProvideReferenceReason(source.Value) : null;

  private ecer_likertscales? MapLikertScale(LikertScale? source) => source.HasValue ? MapLikertScale(source.Value) : null;

  private ecer_ReferenceRelationships? MapReferenceRelationship(ReferenceRelationship? source) => source.HasValue ? MapReferenceRelationship(source.Value) : null;

  private ecer_ChildrenProgramTypes? MapChildrenProgramType(ChildrenProgramType? source) => source.HasValue ? MapChildrenProgramType(source.Value) : null;

  private ecer_WorkHoursType? MapWorkHoursType(WorkHoursType? source) => source.HasValue ? MapWorkHoursType(source.Value) : null;

  private static ecer_WorkExperienceTypes? MapWorkExperienceType(WorkExperienceTypes? source) => source.HasValue ? MapWorkExperienceType(source.Value) : null;

  private static ecer_WorkExperienceTypes MapWorkExperienceType(WorkExperienceTypes source) => source switch
  {
    WorkExperienceTypes.Is500Hours => ecer_WorkExperienceTypes._500Hours,
    WorkExperienceTypes.ICRA => ecer_WorkExperienceTypes.ICRA,
    _ => ecer_WorkExperienceTypes._400Hours,
  };

  private static WorkExperienceTypes MapWorkExperienceType(ecer_WorkExperienceTypes source) => source switch
  {
    ecer_WorkExperienceTypes._500Hours => WorkExperienceTypes.Is500Hours,
    ecer_WorkExperienceTypes.ICRA => WorkExperienceTypes.ICRA,
    _ => WorkExperienceTypes.Is400Hours,
  };
}
