using ECER.Utilities.ObjectStorage.Providers;
using Mediator;

namespace ECER.Managers.Registry.Contract.InvestigationReconsiderations;

public record InvestigationReconsiderationQueryCommand() : IRequest<InvestigationReconsiderationQueryResults>
{
  public string? ById { get; set; }
  public string? ByApplicantId { get; set; }
  public string? ByApplicationId { get; set; }
  public IEnumerable<InvestigationReconsiderationStatusCode>? ByStatusCodes { get; set; }
};

public record InvestigationReconsiderationSubmitCommand(InvestigationReconsideration InvestigationReconsideration, string ApplicantId) : IRequest<InvestigationReconsiderationSubmitResult>
{
};

public record InvestigationReconsiderationQueryResults(IEnumerable<InvestigationReconsideration> Items);

public record InvestigationReconsiderationSubmitResult()
{
  public string? Id { get; set; }
  public bool IsSuccess { get; set; }
  public InvestigationReconsiderationSubmitErrorCode? ErrorCode { get; set; }
};

public enum InvestigationReconsiderationSubmitErrorCode
{
  InvestigationReconsiderationNotFound,
  InvestigationReconsiderationWrongStatus,
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
