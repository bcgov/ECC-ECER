using MediatR;

namespace ECER.Managers.Registry.Contract.ICRA;

public record SaveICRAEligibilityCommand(ICRAEligibility eligibility) : IRequest<Contract.ICRA.ICRAEligibility?>;

public record ICRAEligibilitiesQuery : IRequest<ICRAEligibilitiesQueryResults>
{
  public string? ById { get; set; }
  public string? ByApplicantId { get; set; }
  public string? PortalStage { get; set; }
  public IEnumerable<ICRAStatus>? ByStatus { get; set; }
}

public record ICRAEligibilitiesQueryResults(IEnumerable<ICRAEligibility> Items);

public record ICRAEligibility()
{
  public string? Id { get; set; }
  public string? PortalStage { get; set; }
  public string ApplicantId { get; set; } = string.Empty;
  public ICRAStatus Status { get; set; }
}

public enum ICRAStatus
{
  Active,
  Draft,
  Eligible,
  Inactive,
  Ineligible,
  InReview,
  ReadyforReview,
  Submitted
}
