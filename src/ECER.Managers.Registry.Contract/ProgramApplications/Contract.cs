using MediatR;

namespace ECER.Managers.Registry.Contract.ProgramApplications;

public record CreateProgramApplicationCommand(ProgramApplication ProgramApplication) : IRequest<ProgramApplication?>;

public record ProgramApplicationQuery : IRequest<ProgramApplicationQueryResults>
{
  public string? ById { get; set; }
  public string? ByPostSecondaryInstituteId { get; set; }
  public IEnumerable<ApplicationStatus>? ByStatus { get; set; }
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
}

public record UpdateProgramApplicationCommand(ProgramApplication ProgramApplication) : IRequest<string>;

public record ProgramApplication(string? Id, string PostSecondaryInstituteId)
{
  public string? ProgramApplicationName { get; set; }
  public ApplicationType? ProgramApplicationType { get; set; }
  public ApplicationStatus? Status { get; set; }
  public IEnumerable<ProgramCertificationType>? ProgramTypes { get; set; }
  public DeliveryType? DeliveryType { get; set; }
  public bool? ComponentsGenerationCompleted { get; set; }
}

public record ProgramApplicationQueryResults(IEnumerable<ProgramApplication> Items, int Count);
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
  RFAI,
  SiteVisitRequired,
  Submitted,
  Withdrawn
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
