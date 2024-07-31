using ECER.Resources.Documents.PortalInvitations;

namespace ECER.Resources.Documents.Applications;

public interface IApplicationRepository
{
  Task<IEnumerable<Application>> Query(ApplicationQuery query, CancellationToken cancellationToken);

  Task<string> SaveDraft(Application application, CancellationToken cancellationToken);

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
  public PortalStage Stage { get; set; }
  public IEnumerable<Transcript> Transcripts { get; set; } = Array.Empty<Transcript>();
  public IEnumerable<ProfessionalDevelopment> ProfessionalDevelopments { get; set; } = Array.Empty<ProfessionalDevelopment>();
  public IEnumerable<WorkExperienceReference> WorkExperienceReferences { get; set; } = Array.Empty<WorkExperienceReference>();
  public IEnumerable<CharacterReference> CharacterReferences { get; set; } = Array.Empty<CharacterReference>();
  public DateTime? ReadyForAssessmentDate { get; set; }
  public bool? AddMoreCharacterReference { get; set; }
  public bool? AddMoreWorkExperienceReference { get; set; }
  public string? ExplanationLetter { get; set; }
  public OneYearRenewalexplanations OneYearRenewalexplanation { get; set; }
}

public record Transcript(string? Id, string? EducationalInstitutionName, string? ProgramName, string? StudentName, string? StudentNumber, DateTime StartDate, DateTime EndDate, bool IsECEAssistant, bool DoesECERegistryHaveTranscript, bool IsOfficialTranscriptRequested)
{
  public string? CampusLocation { get; set; }
  public string? LanguageofInstruction { get; set; }
  public TranscriptStage? Status { get; set; }
}

public record ProfessionalDevelopment(string? Id, string? CertificationNumber, DateTime CertificationExpiryDate, DateTime DateSigned, string? CourseName, string? OrganizationName, DateTime StartDate, DateTime EndDate)
{
  public string? OrganizationContactInformation { get; set; }
  public string? InstructorName { get; set; }
  public int? NumberOfHours { get; set; }
  public ProfessionalDevelopmentStatusCode? Status { get; set; }
}

public record WorkExperienceReference(string? FirstName, string? LastName, string? EmailAddress, int? Hours)
{
  public string? Id { get; set; }
  public string? PhoneNumber { get; set; }
  public WorkExperienceRefStage? Status { get; set; }
  public bool? WillProvideReference { get; set; }
  public int? TotalNumberofHoursApproved { get; set; }
  public int? TotalNumberofHoursObserved { get; set; }
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

public enum OneYearRenewalexplanations
{
  Icouldnotfindemploymenttocompletetherequiredhours,
  Icouldnotworkduetomyvisastatusstudentvisaexpiredvisa,
  IliveandworkinacommunitywithoutothercertifiedECEs,
  Iwasunabletoenterthecountryasexpected,
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
public record WorkExperienceReferenceDetails(int Hours, WorkHoursType WorkHoursType, string ChildrenProgramName, ChildrenProgramType ChildrenProgramType, string ChildrenProgramTypeOther, IEnumerable<ChildcareAgeRanges> ChildcareAgeRanges, DateTime StartDate, DateTime EndDate, ReferenceRelationship ReferenceRelationship, string ReferenceRelationshipOther);
public record WorkExperienceReferenceCompetenciesAssessment(LikertScale ChildDevelopment, string ChildDevelopmentReason, LikertScale ChildGuidance, string ChildGuidanceReason, LikertScale HealthSafetyAndNutrition, string HealthSafetyAndNutritionReason, LikertScale DevelopAnEceCurriculum, string DevelopAnEceCurriculumReason, LikertScale ImplementAnEceCurriculum, string ImplementAnEceCurriculumReason, LikertScale FosteringPositiveRelationChild, string FosteringPositiveRelationChildReason, LikertScale FosteringPositiveRelationFamily, string FosteringPositiveRelationFamilyReason, LikertScale FosteringPositiveRelationCoworker, string FosteringPositiveRelationCoworkerReason);

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

public enum ProfessionalDevelopmentStatusCode
{
  ApplicationSubmitted,
  Draft,
  Inactive,
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
