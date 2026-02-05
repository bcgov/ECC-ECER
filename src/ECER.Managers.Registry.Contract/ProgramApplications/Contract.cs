using MediatR;

namespace ECER.Managers.Registry.Contract.ProgramApplications;

public record ProgramApplicationQuery : IRequest<ProgramApplicationQueryResults>
{
  public string? ById { get; set; }
  public string? ByPostSecondaryInstituteId { get; set; }
  public IEnumerable<ApplicationStatus>? ByStatus { get; set; }
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
}

public record ProgramApplication(string? Id, string PostSecondaryInstituteId)
{
  public string? ProgramApplicationName { get; set; }
  public ApplicationType? ProgramApplicationType { get; set; }
  public ApplicationStatus? Status { get; set; }
  public ProvincialCertificationTypeOffered? ProgramType { get; set; }
  public DeliveryType? DeliveryType { get; set; }
}

public record ProgramApplicationQueryResults(IEnumerable<ProgramApplication> Items, int Count);
public enum ApplicationStatus
{
  Draft,
  InterimRecognition,
  OnGoingRecognition,
  PendingReview,
  RefusetoApprove,
  ReviewAnalysis,
  RFAI,
  Submitted,
  Withdrawn
}

public enum ApplicationType
{
  NewBasicPostBasicProgramHybridOnline,
  NewBasicPostBasicProgramInperson,
  NewDeliveryMethod,
  PrivateNewCampusLocation,
  SatelliteProgram,
}

public enum DeliveryType
{
  Hybrid,
  Inperson,
  Online,
  Satellite,
  WorkIntegratedLearning
}

public enum ProvincialCertificationTypeOffered
{
  ECEBasic,
  ITE,
  ITESNE,
  SNE
}
