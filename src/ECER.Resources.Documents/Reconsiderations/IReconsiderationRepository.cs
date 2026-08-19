using ECER.Utilities.ObjectStorage.Providers;

namespace ECER.Resources.Documents.Reconsiderations;

public interface IReconsiderationRepository
{
  Task<IEnumerable<Reconsideration>> Query(ReconsiderationQuery query, CancellationToken cancellationToken);

  Task<string> Submit(Reconsideration reconsideration, string applicantId, CancellationToken cancellationToken);
}

public record ReconsiderationQuery
{
  public string? ById { get; set; }
  public string? ByApplicantId { get; set; }
  public string? ByApplicationId { get; set; }
  public IEnumerable<ReconsiderationStatusCode>? ByStatusCodes { get; set; }
}

public record Reconsideration()
{
  public string? Id { get; set; }
  public string? ReconsiderationDetails { get; set; }
  public string? ExplanationAndEvidence { get; set; }
  public ReconsiderationStatusCode Status { get; set; }
  public IEnumerable<FileInfo> Files { get; set; } = Array.Empty<FileInfo>();
  public DateTime? ReconsiderationEndDate { get; set; }
  public string? ApplicationId { get; set; }
}
public record FileInfo(string Id)
{
  public string? Url { get; set; } = string.Empty;
  public string? Extention { get; set; } = string.Empty;
  public string? Name { get; set; } = string.Empty;
  public string? Size { get; set; } = string.Empty;
  public EcerWebApplicationType EcerWebApplicationType { get; set; }
}

public enum ReconsiderationStatusCode
{
  Complete,
  InReview,
  New,
}
