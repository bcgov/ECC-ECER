namespace ECER.Resources.Documents.ProgramApplications;

public interface IProgramApplicationRepository
{
  Task<string> Create(ProgramApplication programApplication, CancellationToken cancellationToken);
  Task<ProgramApplicationQueryResults> Query(ProgramApplicationQuery query, CancellationToken cancellationToken);
  Task<string> UpdateProgramApplication(ProgramApplication application, CancellationToken cancellationToken);
  Task<IEnumerable<ComponentGroupMetadata>> QueryComponentGroups(ComponentGroupQuery query, CancellationToken cancellationToken);
  Task<ComponentGroupResults?> QueryComponentGroupById(ComponentGroupWithComponentsQuery query, CancellationToken cancellationToken);
}

public record ComponentGroupResults(string Id, string Name, string? Instruction, string Status, string CategoryName, int DisplayOrder, IEnumerable<ProgramApplicationComponent> Components);

public record ComponentGroupMetadata(string Id, string Name, string Status, string CategoryName, int DisplayOrder);
public record ComponentGroupQuery
{
  public string? ByProgramApplicationId { get; set; }
}

public record ComponentGroupWithComponentsQuery
{
  public string? ByProgramApplicationId { get; set; }
  public string? ByComponentGroupId { get; set; }
}

public record ProgramApplicationComponent(string Id, string Name, string? Question, int DisplayOrder, string? Answer, IEnumerable<string>? FileIds);

public record ProgramApplicationQuery
{
  public string? ById { get; set; }
  public string? ByPostSecondaryInstituteId { get; set; }
  public IEnumerable<ApplicationStatus>? ByStatus { get; set; }
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
}

public record ProgramApplicationQueryResults(IEnumerable<ProgramApplication> Items, int Count);

public record ProgramApplication(string? Id, string PostSecondaryInstituteId)
{
  public string? ProgramApplicationName { get; set; }
  public ApplicationType? ProgramApplicationType { get; set; }
  public ApplicationStatus? Status { get; set; }
  public ApplicationStatusReasonDetail? StatusReasonDetail { get; set; }
  public IEnumerable<ProgramCertificationType>? ProgramTypes { get; set; }
  public DeliveryType? DeliveryType { get; set; }
  public bool? ComponentsGenerationCompleted { get; set; }
}
public enum ApplicationStatus
{
  Denied,
  Draft,
  Inactive,
  InterimRecognition,
  OnGoingRecognition,
  PendingDecision,
  PendingReview,
  RefusetoApprove,
  ReviewAnalysis,
  SiteVisitRequired,
  Submitted,
  Withdrawn
}

public enum ApplicationStatusReasonDetail
{
  Pendingdecision,
  Recognitionevaluationmeeting,
  RFAIreceived,
  RFAIrequested,
}

public enum ApplicationType
{
  AddOnlineorHybridDeliveryMethod,
  CurriculumRevisionsatRecognizedInstitution,
  NewBasicECEPostBasicProgram,
  NewCampusatRecognizedPrivateInstitution,
  SatelliteProgram,
  WorkIntegratedLearningProgram,
}

public enum DeliveryType
{
  Hybrid,
  Inperson,
  Online,
}

public enum ProvincialCertificationTypeOffered
{
  ECEBasic,
  ITE,
  ITESNE,
  SNE
}

public enum ProgramCertificationType
{
  Basic,
  ITE,
  SNE
}
