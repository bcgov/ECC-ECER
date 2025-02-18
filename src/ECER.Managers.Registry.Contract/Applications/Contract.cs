using MediatR;

namespace ECER.Managers.Registry.Contract.Applications;

/// <summary>
/// Invokes draft application saving use case
/// </summary>
public record SaveDraftApplicationCommand(Application Application) : IRequest<Contract.Applications.Application?>;

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
  public IEnumerable<ProfessionalDevelopment> ProfessionalDevelopments { get; set; } = Array.Empty<ProfessionalDevelopment>();
  public IEnumerable<WorkExperienceReference> WorkExperienceReferences { get; set; } = Array.Empty<WorkExperienceReference>();
  public IEnumerable<CharacterReference> CharacterReferences { get; set; } = Array.Empty<CharacterReference>();
  public string? Stage { get; set; }
  public ApplicationStatusReasonDetail SubStatus { get; set; }
  public DateTime? ReadyForAssessmentDate { get; set; }
  public bool? AddMoreCharacterReference { get; set; }
  public bool? AddMoreWorkExperienceReference { get; set; }
  public bool? AddMoreProfessionalDevelopment { get; set; }
  public ApplicationTypes ApplicationType { get; set; }
  public EducationOrigin? EducationOrigin { get; set; }
  public EducationRecognition? EducationRecognition { get; set; }
  public OneYearRenewalexplanations? OneYearRenewalExplanationChoice { get; set; }
  public FiveYearRenewalExplanations? FiveYearRenewalExplanationChoice { get; set; }
  public string? RenewalExplanationOther { get; set; }
  public ApplicationOrigin? Origin { get; set; }
}

public record Transcript(string? Id, string? EducationalInstitutionName, string? ProgramName, string? StudentNumber, DateTime StartDate, DateTime EndDate, bool IsECEAssistant, bool DoesECERegistryHaveTranscript, bool IsOfficialTranscriptRequested, string StudentFirstName, string StudentLastName, bool IsNameUnverified, EducationRecognition EducationRecognition, EducationOrigin EducationOrigin)
{
  public string? CampusLocation { get; set; }
  public TranscriptStage? Status { get; set; }
  public string? StudentMiddleName { get; set; }
}

public record WorkExperienceReference(string? FirstName, string? LastName, string? EmailAddress, int? Hours)
{
  public string? Id { get; set; }
  public string? PhoneNumber { get; set; }
  public WorkExperienceRefStage? Status { get; set; }
  public bool? WillProvideReference { get; set; }
  public int? TotalNumberofHoursApproved { get; set; }
  public int? TotalNumberofHoursObserved { get; set; }
  public WorkExperienceTypes? Type { get; set; }
}
public record ProfessionalDevelopment(string? Id, string? CourseName, string? OrganizationName, DateTime StartDate, DateTime EndDate)
{
  public string? CourseorWorkshopLink { get; set; }
  public string? OrganizationContactInformation { get; set; }
  public string? OrganizationEmailAddress { get; set; }
  public string? InstructorName { get; set; }
  public double? NumberOfHours { get; set; }
  public ProfessionalDevelopmentStatusCode? Status { get; set; }
  public IEnumerable<string> DeletedFiles { get; set; } = Array.Empty<string>();
  public IEnumerable<string> NewFiles { get; set; } = Array.Empty<string>();
  public IEnumerable<FileInfo> Files { get; set; } = Array.Empty<FileInfo>();
}

public record FileInfo(string Id)
{
  public string? Url { get; set; } = string.Empty;
  public string? Extention { get; set; } = string.Empty;
  public string? Name { get; set; } = string.Empty;
  public string? Size { get; set; } = string.Empty;
}
public record CharacterReference(string? FirstName, string? LastName, string? PhoneNumber, string? EmailAddress)
{
  public string? Id { get; set; }
  public CharacterReferenceStage? Status { get; set; }
  public bool? WillProvideReference { get; set; }
}

public enum ProfessionalDevelopmentStatusCode
{
  ApplicationSubmitted,
  Approved,
  Draft,
  InProgress,
  Rejected,
  Submitted,
  UnderReview,
  WaitingResponse,
}

public enum CertificationType
{
  EceAssistant,
  OneYear,
  FiveYears,
  Ite,
  Sne
}

public enum FiveYearRenewalExplanations
{
  Ileftthechildcarefieldforpersonalreasons,
  Iwasunabletocompletetherequiredhoursofprofessionaldevelopment,
  Iwasunabletofindemploymentinthechildcarefieldinmycommunity,
  MyemploymentdiddoesnotrequirecertificationasanECEforexamplenannyteachercollegeinstructoradministratoretc,
  Other,
}

public enum OneYearRenewalexplanations
{
  IliveandworkinacommunitywithoutothercertifiedECEs,
  Iwasunabletofindemploymentinthechildcarefieldtocompletetherequirednumberofhours,
  Iwasunabletoworkduetothestatusofmyvisaorwasunabletoenterthecountryasexpected,
  Iwasunabletoworkinthechildcarefieldforpersonalreasons,
  Other,
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
  ReconsiderationDecision,
  AppealDecision
}

public enum ApplicationOrigin
{
  Manual,
  Oracle,
  Portal
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

public enum ApplicationTypes
{
  New,
  Renewal,
  LaborMobility
}

public enum EducationOrigin
{
  InsideBC,
  OutsideBC,
  OutsideofCanada
}

public enum EducationRecognition
{
  Recognized,
  NotRecognized
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
public record WorkExperienceReferenceDetails()
{
  public int? Hours { get; set; }
  public WorkHoursType? WorkHoursType { get; set; }
  public string? ChildrenProgramName { get; set; }
  public ChildrenProgramType? ChildrenProgramType { get; set; }
  public string? ChildrenProgramTypeOther { get; set; }
  public IEnumerable<ChildcareAgeRanges>? ChildcareAgeRanges { get; set; }
  public string? Role { get; set; }
  public string? AgeofChildrenCaredFor { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public ReferenceRelationship? ReferenceRelationship { get; set; }
  public string? ReferenceRelationshipOther { get; set; }
  public string? AdditionalComments { get; set; }
  public WorkExperienceTypes? WorkExperienceType { get; set; }
}

public record WorkExperienceReferenceCompetenciesAssessment()
{
  public LikertScale? ChildDevelopment { get; set; }
  public string? ChildDevelopmentReason { get; set; }
  public LikertScale? ChildGuidance { get; set; }
  public string? ChildGuidanceReason { get; set; }
  public LikertScale? HealthSafetyAndNutrition { get; set; }
  public string? HealthSafetyAndNutritionReason { get; set; }
  public LikertScale? DevelopAnEceCurriculum { get; set; }
  public string? DevelopAnEceCurriculumReason { get; set; }
  public LikertScale? ImplementAnEceCurriculum { get; set; }
  public string? ImplementAnEceCurriculumReason { get; set; }
  public LikertScale? FosteringPositiveRelationChild { get; set; }
  public string? FosteringPositiveRelationChildReason { get; set; }
  public LikertScale? FosteringPositiveRelationFamily { get; set; }
  public string? FosteringPositiveRelationFamilyReason { get; set; }
  public LikertScale? FosteringPositiveRelationCoworker { get; set; }
  public string? FosteringPositiveRelationCoworkerReason { get; set; }
}

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

public record UpdateWorkExperienceReferenceCommand(WorkExperienceReference workExperienceRef, string applicationId, string referenceId, string userId) : IRequest<UpdateWorkExperienceReferenceResult>;

public class UpdateWorkExperienceReferenceResult
{
  public string? ReferenceId { get; set; }
  public bool IsSuccess { get; set; }
  public string? ErrorMessage { get; set; }

  public static UpdateWorkExperienceReferenceResult Success() => new UpdateWorkExperienceReferenceResult { IsSuccess = true };

  public static UpdateWorkExperienceReferenceResult Failure(string message) => new UpdateWorkExperienceReferenceResult { IsSuccess = false, ErrorMessage = message };
}

public record UpdateCharacterReferenceCommand(CharacterReference characterRef, string applicationId, string referenceId, string userId) : IRequest<UpdateCharacterReferenceResult>;

public class UpdateCharacterReferenceResult
{
  public string? ReferenceId { get; set; }
  public bool IsSuccess { get; set; }
  public string? ErrorMessage { get; set; }

  public static UpdateCharacterReferenceResult Success() => new UpdateCharacterReferenceResult { IsSuccess = true };

  public static UpdateCharacterReferenceResult Failure(string message) => new UpdateCharacterReferenceResult { IsSuccess = false, ErrorMessage = message };
}

public record AddProfessionalDevelopmentCommand(ProfessionalDevelopment professionalDevelopment, string applicationId, string userId) : IRequest<AddProfessionalDevelopmentResult>;

public class AddProfessionalDevelopmentResult
{
  public string? ApplicationId { get; set; }
  public bool IsSuccess { get; set; }
  public string? ErrorMessage { get; set; }
}

public enum ReferenceKnownTime
{
  From1to2years,
  From2to5years,
  From6monthsto1year,
  Lessthan6months,
  Morethan5years,
}

public enum TranscriptStage
{
  Accepted,
  ApplicationSubmitted,
  Draft,
  InProgress,
  Rejected,
  Submitted,
  WaitingforDetails
}

public enum WorkExperienceRefStage
{
  ApplicationSubmitted,
  Approved,
  Draft,
  InProgress,
  Rejected,
  Submitted,
  UnderReview,
  WaitingforResponse
}

public enum WorkExperienceTypes
{
  Is400Hours,
  Is500Hours,
}

public enum CharacterReferenceStage
{
  ApplicationSubmitted,
  Approved,
  Draft,
  InProgress,
  Rejected,
  Submitted,
  UnderReview,
  WaitingResponse
}
