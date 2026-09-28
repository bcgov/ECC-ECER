using ECER.Utilities.ObjectStorage.Providers;

namespace ECER.Resources.Documents.InvestigationReconsiderations;

public interface IInvestigationReconsiderationRepository
{
  Task<IEnumerable<InvestigationReconsideration>> Query(InvestigationReconsiderationQuery query, CancellationToken cancellationToken);

  Task<string> Submit(InvestigationReconsideration investigationReconsideration, string applicantId, CancellationToken cancellationToken);
}

public record InvestigationReconsiderationQuery
{
  public string? ById { get; set; }
  public string? ByApplicantId { get; set; }
  public IEnumerable<InvestigationReconsiderationStatusCode>? ByStatusCodes { get; set; }
}

public record InvestigationReconsideration()
{
  public string? Id { get; set; }
  public string? ExplanationAndEvidence { get; set; }
  public DateTime? ReconsiderationEndDate { get; set; }
  public InvestigationReconsiderationStatusCode Status { get; set; }
  public InvestigationReconsiderationType Type { get; set; }
  public IEnumerable<FileInfo> Files { get; set; } = Array.Empty<FileInfo>();
}
public record FileInfo(string Id)
{
  public string? Url { get; set; } = string.Empty;
  public string? Extention { get; set; } = string.Empty;
  public string? Name { get; set; } = string.Empty;
  public string? Size { get; set; } = string.Empty;
  public EcerWebApplicationType EcerWebApplicationType { get; set; }
}

public enum InvestigationReconsiderationStatusCode
{
  Complete,
  InReview,
  New,
}

public enum InvestigationReconsiderationType
{
  InvestigationOutcome,
  ImmediateAction,
}
