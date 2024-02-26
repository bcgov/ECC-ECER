namespace ECER.Resources.Documents.Applications;

public interface IApplicationRepository
{
  Task<IEnumerable<Application>> Query(ApplicationQuery query);

  Task<string> SaveDraft(Application application);

  Task<string> Submit(string applicationId);
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
  public DateTime CreatedOn { get; set; }
  public DateOnly SignedDate { get; set; }
  public DateTime? SubmittedOn { get; set; }
  public PortalStage Stage { get; set; }
}

public enum PortalStage
{
  ContactInformation,
  Education,
  References,
  Review,
  Declaration,
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
  Decision,
  Escalated,
  InProgress,
  Pending,
  PendingQueue,
  Ready,
  ReconsiderationDecision,
  Withdrawn,
}
