namespace ECER.Resources.Documents.ICRA;

public interface IICRARepository
{
  Task<IEnumerable<ICRAEligibility>> Query(ICRAQuery query, CancellationToken cancellationToken);

  Task<string> Save(ICRAEligibility iCRAEligibility, CancellationToken cancellationToken);
}

public record ICRAQuery
{
  public string? ById { get; set; }
  public IEnumerable<ICRAStatus>? ByStatus { get; set; }
  public string? ByApplicantId { get; set; }
}

public record ICRAEligibility(string? Id)
{
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
