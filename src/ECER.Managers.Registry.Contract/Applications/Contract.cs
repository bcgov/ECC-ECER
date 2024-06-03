using MediatR;

namespace ECER.Managers.Registry.Contract.Applications;

/// <summary>
/// Invokes draft application saving use case
/// </summary>
public record SaveDraftApplicationCommand(Application Application) : IRequest<string>;

/// <summary>
/// Invokes draft application saving use case
/// </summary>
public record CancelDraftApplicationCommand(string applicationId, string userId) : IRequest<string>;

/// <summary>
/// Invokes application submission use case
/// </summary>
/// <param name="applicationId"></param>
/// <param name="userId"></param>
public record SubmitApplicationCommand(string applicationId, string userId) : IRequest<ApplicationSubmissionResult>;

/// <summary>
/// Invokes application query use case
/// </summary>
public record ApplicationsQuery : IRequest<ApplicationsQueryResults>
{
  public string? ById { get; set; }
  public string? ByApplicantId { get; set; }
  public IEnumerable<ApplicationStatus>? ByStatus { get; set; }
}

/// <summary>
/// Container for <see cref="ApplicationsQuery"/> results
/// </summary>
/// <param name="Items">The </param>
public record ApplicationsQueryResults(IEnumerable<Application> Items);

/// <summary>
/// Application submission result
/// </summary>
public record ApplicationSubmissionResult()
{
  public string? ApplicationId { get; set; }
  public SubmissionError? Error { get; set; }
  public IEnumerable<string>? ValidationErrors { get; set; }
  public bool IsSuccess { get { return ValidationErrors == null || !ValidationErrors.Any(); } }
}

public record Application(string? Id, string RegistrantId, ApplicationStatus Status)
{
  public DateTime? SubmittedOn { get; set; }
  public DateTime? CreatedOn { get; set; }
  public DateTime? SignedDate { get; set; }
  public IEnumerable<CertificationType> CertificationTypes { get; set; } = Array.Empty<CertificationType>();
  public IEnumerable<Transcript> Transcripts { get; set; } = Array.Empty<Transcript>();
  public IEnumerable<WorkExperienceReference> WorkExperienceReferences { get; set; } = Array.Empty<WorkExperienceReference>();
  public PortalStage Stage { get; set; }
  public IEnumerable<CharacterReference> CharacterReferences { get; set; } = Array.Empty<CharacterReference>();
  public ApplicationStatusReasonDetail SubStatus { get; set; }
  public DateTime? ReadyForAssessmentDate { get; set; }
}

public record Transcript(string? Id, string? EducationalInstitutionName, string? ProgramName, string? StudentName, string? StudentNumber, DateTime StartDate, DateTime EndDate, bool IsECEAssistant, bool DoesECERegistryHaveTranscript, bool IsOfficialTranscriptRequested)
{
  public string? CampusLocation { get; set; }
  public string? LanguageofInstruction { get; set; }
  public StageStatus? Status { get; set; }
}

public record WorkExperienceReference(string? FirstName, string? LastName, string? EmailAddress, int? Hours)
{
  public string? Id { get; set; }

  public string? PhoneNumber { get; set; }

  public StageStatus? Status { get; set; }

  public bool? WillProvideReference { get; set; }
}

public record CharacterReference(string? FirstName, string? LastName, string? PhoneNumber, string? EmailAddress)
{
  public string? Id { get; set; }

  public StageStatus? Status { get; set; }

  public bool? WillProvideReference { get; set; }
}

public enum PortalStage
{
  CertificationType,
  Declaration,
  ContactInformation,
  Education,
  CharacterReferences,
  WorkReferences,
  Review,
}

public enum CertificationType
{
  EceAssistant,
  OneYear,
  FiveYears,
  Ite,
  Sne
}

public enum SubmissionError
{
  DraftApplicationNotFound,
  DraftApplicationValidationFailed
}

public enum ApplicationStatus
{
  Draft,
  Submitted,
  Complete,
  Reconsideration,
  Cancelled,
  Escalated,
  Decision,
  Withdrawn,
  Pending,
  Ready,
  InProgress,
  PendingQueue,
  ReconsiderationDecision
}

public enum ApplicationStatusReasonDetail
{
  Actioned,
  BeingAssessed,
  Certified,
  Denied,
  ForReview,
  InvestigationsConsultationNeeded,
  MoreInformationRequired,
  OperationSupervisorManagerofCertificationsConsultationNeeded,
  PendingDocuments,
  ProgramAnalystReview,
  ReadyforAssessment,
  ReceivedPending,
  ReceivePhysicalTranscripts,
  SupervisorConsultationNeeded,
  ValidatingIDs,
}

public record CharacterReferenceSubmissionRequest(string Token, bool WillProvideReference, ReferenceContactInformation ReferenceContactInformation, CharacterReferenceEvaluation ReferenceEvaluation, bool ConfirmProvidedInformationIsRight);
public record ReferenceContactInformation(string LastName, string FirstName, string Email, string PhoneNumber, string CertificateProvinceOther)
{
  public string? CertificateProvinceId { get; set; }
  public string? CertificateNumber { get; set; }
  public DateTime? DateOfBirth { get; set; }
}
public record CharacterReferenceEvaluation(ReferenceRelationship ReferenceRelationship, string ReferenceRelationshipOther, ReferenceKnownTime LengthOfAcquaintance, bool WorkedWithChildren, string ChildInteractionObservations, string ApplicantTemperamentAssessment);
public record OptOutReferenceRequest(string Token, UnabletoProvideReferenceReasons UnabletoProvideReferenceReasons) : IRequest<ReferenceSubmissionResult>;

public record ResendCharacterReferenceInviteRequest(string ApplicationId, string ReferenceId, string UserId) : IRequest<string>;
public record ResendWorkExperienceReferenceInviteRequest(string ApplicationId, string ReferenceId, string UserId) : IRequest<string>;

public enum UnabletoProvideReferenceReasons
{
  Iamunabletoatthistime,
  Idonothavetheinformationrequired,
  Idonotknowthisperson,
  Idonotmeettherequirementstoprovideareference,
  Other
}

public class ReferenceSubmissionResult
{
  public bool IsSuccess { get; set; }
  public string? ErrorMessage { get; set; }

  public static ReferenceSubmissionResult Success() => new ReferenceSubmissionResult { IsSuccess = true };

  public static ReferenceSubmissionResult Failure(string message) => new ReferenceSubmissionResult { IsSuccess = false, ErrorMessage = message };
}

public record WorkExperienceReferenceSubmissionRequest(string Token, bool WillProvideReference, ReferenceContactInformation ReferenceContactInformation, WorkExperienceReferenceDetails WorkExperienceReferenceDetails, WorkExperienceReferenceCompetenciesAssessment WorkExperienceReferenceCompetenciesAssessment, bool ConfirmProvidedInformationIsRight);
public record WorkExperienceReferenceDetails(int Hours, WorkHoursType WorkHoursType, string ChildrenProgramName, ChildrenProgramType ChildrenProgramType, string ChildrenProgramTypeOther, IEnumerable<ChildcareAgeRanges> ChildcareAgeRanges, DateTime StartDate, DateTime EndDate, ReferenceRelationship ReferenceRelationship, string ReferenceRelationshipOther);
public record WorkExperienceReferenceCompetenciesAssessment(LikertScale ChildDevelopment, string ChildDevelopmentReason, LikertScale ChildGuidance, string ChildGuidanceReason, LikertScale HealthSafetyAndNutrition, string HealthSafetyAndNutritionReason, LikertScale DevelopAnEceCurriculum, string DevelopAnEceCurriculumReason, LikertScale ImplementAnEceCurriculum, string ImplementAnEceCurriculumReason, LikertScale FosteringPositiveRelationChild, string FosteringPositiveRelationChildReason, LikertScale FosteringPositiveRelationFamily, string FosteringPositiveRelationFamilyReason, LikertScale FosteringPositiveRelationCoworker, string FosteringPositiveRelationCoworkerReason);

public enum WorkHoursType
{
  FullTime,
  PartTime,
}

public enum ChildrenProgramType
{
  Childminding,
  Familychildcare,
  Groupchildcare,
  InHomeMultiAgechildcare,
  MultiAgechildcare,
  Occasionalchildcare,
  Other,
  Preschool,
}

public enum ReferenceRelationship
{
  CoWorker,
  Other,
  ParentGuardianofChildinCare,
  Supervisor,
  Teacher,
}

public enum ChildcareAgeRanges
{
  From0to12Months,
  From12to24Months,
  From25to30Months,
  From31to36Months,
  Grade1,
  Preschool,
}

public enum LikertScale
{
  Yes,
  No
}

public record SubmitReferenceCommand(string Token) : IRequest<ReferenceSubmissionResult>
{
  public WorkExperienceReferenceSubmissionRequest? WorkExperienceReferenceSubmissionRequest { get; set; }
  public CharacterReferenceSubmissionRequest? CharacterReferenceSubmissionRequest { get; set; }
}

public enum ReferenceKnownTime
{
  From1to2years,
  From2to5years,
  From6monthsto1year,
  Lessthan6months,
  Morethan5years,
}

public enum StageStatus
{
  Complete,
  InComplete,
  InProgress,
  Rejected
}
