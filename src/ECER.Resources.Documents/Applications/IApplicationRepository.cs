using ECER.Resources.Documents.PortalInvitations;

namespace ECER.Resources.Documents.Applications;

public interface IApplicationRepository
{
  Task<IEnumerable<Application>> Query(ApplicationQuery query, CancellationToken cancellationToken);

  Task<string> SaveApplication(Application application, CancellationToken cancellationToken);

  Task<string> Submit(string applicationId, CancellationToken cancellationToken);

  Task<string> Cancel(string applicationId, CancellationToken cancellationToken);

  Task<string> SubmitReference(SubmitReferenceRequest request, CancellationToken cancellationToken);

  Task<string> OptOutReference(OptOutReferenceRequest request, CancellationToken cancellationToken);

  Task<string> UpdateWorkExReferenceForSubmittedApplication(WorkExperienceReference updatedReference, string applicationId, string referenceId, string userId, CancellationToken cancellationToken);

  Task<string> UpdateCharacterReferenceForSubmittedApplication(CharacterReference updatedReference, string applicationId, string referenceId, string userId, CancellationToken cancellationToken);

  Task<string> ResendCharacterReferenceInvite(ResendReferenceInviteRequest request, CancellationToken cancellationToken);

  Task<string> ResendWorkExperienceReferenceInvite(ResendReferenceInviteRequest request, CancellationToken cancellationToken);
}

public record ApplicationQuery
{
  public string? ById { get; set; }
  public IEnumerable<ApplicationStatus>? ByStatus { get; set; }
  public string? ByApplicantId { get; set; }
}

public record Application(string? Id, string ApplicantId, IEnumerable<CertificationType> CertificationTypes)
{
  public ApplicationStatus Status { get; set; }
  public ApplicationStatusReasonDetail SubStatus { get; set; }
  public DateTime CreatedOn { get; set; }
  public DateTime? SignedDate { get; set; }
  public DateTime? SubmittedOn { get; set; }
  public string? Stage { get; set; }
  public IEnumerable<Transcript> Transcripts { get; set; } = Array.Empty<Transcript>();
  public IEnumerable<ProfessionalDevelopment> ProfessionalDevelopments { get; set; } = Array.Empty<ProfessionalDevelopment>();
  public IEnumerable<WorkExperienceReference> WorkExperienceReferences { get; set; } = Array.Empty<WorkExperienceReference>();
  public IEnumerable<CharacterReference> CharacterReferences { get; set; } = Array.Empty<CharacterReference>();
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
  public string? LanguageofInstruction { get; set; }
  public TranscriptStage? Status { get; set; }
  public string? StudentMiddleName { get; set; }
}

public record ProfessionalDevelopment(string? Id, string? CourseName, string? OrganizationName, DateTime StartDate, DateTime EndDate)
{
  public string? CourseorWorkshopLink { get; set; }
  public string? OrganizationContactInformation { get; set; }
  public string? OrganizationEmailAddress { get; set; }
  public string? InstructorName { get; set; }
  public int? NumberOfHours { get; set; }
  public ProfessionalDevelopmentStatusCode? Status { get; set; }
  public IEnumerable<FileInfo> Files { get; set; } = Array.Empty<FileInfo>();
  public IEnumerable<string> DeletedFiles { get; set; } = Array.Empty<string>();
  public IEnumerable<string> NewFiles { get; set; } = Array.Empty<string>();
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

public record FileInfo(string Id)
{
  public string? Url { get; set; } = string.Empty;
  public string? Extention { get; set; } = string.Empty;
  public string? Name { get; set; } = string.Empty;
  public string? Size { get; set; } = string.Empty;
}

public enum CertificationType
{
  EceAssistant,
  OneYear,
  FiveYears,
  Ite,
  Sne
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

public enum OneYearRenewalexplanations
{
  Ileftthechildcarefieldforpersonalreasons,
  Iwasunabletocompletetherequiredhoursofprofessionaldevelopment,
  Iwasunabletofindemploymentinthechildcarefieldinmycommunity,
  MyemploymentdiddoesnotrequirecertificationasanECEforexamplenannyteachercollegeinstructoradministratoretc,
  Other,
}

public enum FiveYearRenewalExplanations
{
  IliveandworkinacommunitywithoutothercertifiedECEs,
  Iwasunabletofindemploymentinthechildcarefieldtocompletetherequirednumberofhours,
  Iwasunabletoworkduetothestatusofmyvisaorwasunabletoenterthecountryasexpected,
  Iwasunabletoworkinthechildcarefieldforpersonalreasons,
  Other,
}

public record CharacterReference(string? FirstName, string? LastName, string? PhoneNumber, string? EmailAddress)
{
  public string? Id { get; set; }
  public CharacterReferenceStage? Status { get; set; }
  public bool? WillProvideReference { get; set; }
}

public record OptOutReferenceRequest(UnabletoProvideReferenceReasons UnabletoProvideReferenceReasons)
{
  public PortalInvitation? PortalInvitation { get; set; }
}

public record ResendReferenceInviteRequest(string ReferenceId);

public enum UnabletoProvideReferenceReasons
{
  Iamunabletoatthistime,
  Idonothavetheinformationrequired,
  Idonotknowthisperson,
  Idonotmeettherequirementstoprovideareference,
  Other
}

public record SubmitReferenceRequest()
{
  public PortalInvitation? PortalInvitation { get; set; }
  public DateTime DateSigned { get; set; }
}

public record UpdateWorkExperienceReferenceCommand(WorkExperienceReference workExperienceRef, string applicationId, string userId);
public record CharacterReferenceSubmissionRequest(bool WillProvideReference, ReferenceContactInformation ReferenceContactInformation, CharacterReferenceEvaluation ReferenceEvaluation, bool ConfirmProvidedInformationIsRight) : SubmitReferenceRequest;
public record ReferenceContactInformation(string LastName, string FirstName, string Email, string PhoneNumber, string CertificateProvinceOther)
{
  public string? CertificateProvinceId { get; set; }
  public string? CertificateNumber { get; set; }
  public DateTime? DateOfBirth { get; set; }
}
public record CharacterReferenceEvaluation(ReferenceRelationship ReferenceRelationship, string ReferenceRelationshipOther, ReferenceKnownTime LengthOfAcquaintance, bool WorkedWithChildren, string ChildInteractionObservations, string ApplicantTemperamentAssessment);

public record WorkExperienceReferenceSubmissionRequest(bool WillProvideReference, ReferenceContactInformation ReferenceContactInformation, WorkExperienceReferenceDetails WorkExperienceReferenceDetails, WorkExperienceReferenceCompetenciesAssessment WorkExperienceReferenceCompetenciesAssessment, bool ConfirmProvidedInformationIsRight) : SubmitReferenceRequest;

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

public enum LikertScale
{
  Yes,
  No
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

public enum ReferenceKnownTime
{
  From1to2years,
  From2to5years,
  From6monthsto1year,
  Lessthan6months,
  Morethan5years,
}
