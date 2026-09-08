using ECER.Utilities.ObjectStorage.Providers;
using Mediator;

namespace ECER.Managers.Registry.Contract.Reconsiderations;

public record ReconsiderationQueryCommand() : IRequest<ReconsiderationQueryResults>
{
  public string? ById { get; set; }
  public string? ByApplicantId { get; set; }
  public string? ByApplicationId { get; set; }
  public IEnumerable<ReconsiderationStatusCode>? ByStatusCodes { get; set; }
};

public record ReconsiderationSubmitCommand(Reconsideration Reconsideration, string ApplicantId) : IRequest<ReconsiderationSubmitResult>
{
};

public record ReconsiderationQueryResults(IEnumerable<Reconsideration> Items);

public record ReconsiderationSubmitResult()
{
  public string? Id { get; set; }
  public bool IsSuccess { get; set; }
  public ReconsiderationSubmitErrorCode? ErrorCode { get; set; }
};

public enum ReconsiderationSubmitErrorCode
{
  ReconsiderationNotFound,
  ReconsiderationWrongStatus,
  ApplicationNotFound,
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
